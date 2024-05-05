namespace CancellationTokenExample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //CompleteTaskReturn();
            CompleteTaskThrowIfCancellationRequestedMethod();
        }

        /// <summary>
        /// Use return operator to exit the task execution. 
        /// In this case, the state of the task will be TaskStatus.RunToCompletion.
        /// </summary>
        public static void CompleteTaskReturn()
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            Task task = new Task(() =>
            {
                for (int i = 1; i < 100; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Operation is canceled");
                        return;
                    }
                    Console.WriteLine($"Count is equal to '{i}'");
                    //add some timeout to emulate real-life execution
                    Thread.Sleep(10);
                }
            }, token);
            task.Start();

            // add some timeout to emulate real-life execution
            Thread.Sleep(100);
            // cancel the parallel operation
            cancelTokenSource.Cancel();
            // wait till the operation is completed
            task.Wait();
            // check the operation status
            Console.WriteLine($"Task Status is equal to '{task.Status}'");
            // release resources
            cancelTokenSource.Dispose();
        }

        /// <summary>
        /// Throw OperationCanceledException type exception via ThrowIfCancellationRequested() method call. 
        /// In this case, the state of the task will be TaskStatus.Canceled.
        /// </summary>
        public static void CompleteTaskThrowIfCancellationRequestedMethod()
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            Task task = new Task(() =>
            {
                for (int i = 1; i < 100; i++)
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();
                    Console.WriteLine($"Count is equal to '{i}'");
                    //add some timeout to emulate real-life execution
                    Thread.Sleep(10);
                }
            }, token);

            try
            {
                task.Start();
                // add some timeout to emulate real-life execution
                Thread.Sleep(100);
                // cancel the parallel operation
                cancelTokenSource.Cancel();
                // wait till the operation is completed
                task.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (Exception e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException)
                        Console.WriteLine("Operation is canceled");
                    else
                        Console.WriteLine(e.Message);
                }
            }
            finally
            {
                // release resources
                cancelTokenSource.Dispose();
            }

            // check the operation status
            Console.WriteLine($"Task Status is equal to '{task.Status}'");
        }
    }
}
