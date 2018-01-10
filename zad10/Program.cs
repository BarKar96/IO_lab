using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace zad10
{
    public delegate void MathMulCompletedEventHandler(object sender, MathMulCompletedEventArgs e);

    public class MathMulCompletedEventArgs : AsyncCompletedEventArgs
    {
        public double[][] mat1;
        public double[][] mat2;
        public double[][] mat_res;
        public MathMulCompletedEventArgs(double[][] mat1, double[][] mat2, double[][] mat_res, Exception ex, bool canceled, object userState) : base(ex, canceled, userState)
        {
            this.mat1 = mat1;
            this.mat2 = mat2;
            this.mat_res = mat_res;
        }
    }

    public class MathMul
    {

        //delegate will execute main worker method asynchronously
        private delegate void WorkerEventHandler(double[][] mat1, double[][] mat2, AsyncOperation asyncOp);
        public delegate void ProgressChangedEventHandler(ProgressChangedEventArgs e);
        //This delegate raise the event post completing the async operation.
        private SendOrPostCallback onCompletedDelegate;
        private SendOrPostCallback onProgressReportDelegate;
        //To allow async method to call multiple time, We need to store tasks in the list
        //so we can send back the proper value back to main thread
        private HybridDictionary tasks = new HybridDictionary();
        public event ProgressChangedEventHandler ProgressChanged;
        //Event will we captured by the main thread.
        public event MathMulCompletedEventHandler MathMulCompleted;
        private HybridDictionary userStateToLifetime = new HybridDictionary();
        public static int matSize = 10;
        private static Object thisLock = new Object();
        protected virtual void InitializeDelegates()
        {
            onProgressReportDelegate = new SendOrPostCallback(ReportProgress);
            onCompletedDelegate = new SendOrPostCallback(CalculateCompleted);
        }
        public MathMul()
        {
            onCompletedDelegate = new SendOrPostCallback(CalculateCompleted);

        }
        private void ReportProgress(object state)
        {
            ProgressChangedEventArgs e =
                state as ProgressChangedEventArgs;

            OnProgressChanged(e);
        }
        protected void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(e);
            }
        }
        /// <summary>
        /// This function will be called by SendOrPostCallback to raise Method1Completed Event
        /// </summary>
        /// <param name="operationState">Method1CompletedEventArgs object</param>

        private void CalculateCompleted(object operationState)
        {
            MathMulCompletedEventArgs e = operationState as MathMulCompletedEventArgs;
            if (MathMulCompleted != null)
            {
                MathMulCompleted(this, e);
            }
        }

        public void CancelAsync(object taskId)
        {
            AsyncOperation asyncOp = tasks[taskId] as AsyncOperation;
            if (asyncOp != null)
            {
                lock (tasks.SyncRoot)
                {
                    tasks.Remove(taskId);
                }
            }
        }
        private void CompletionMethod(double[][] mat1, double[][] mat2, double[][] mat_res, Exception exception, bool canceled, AsyncOperation asyncOp)
        {
            // If the task was not previously canceled,
            // remove the task from the lifetime collection.
            if (!canceled)
            {
                lock (userStateToLifetime.SyncRoot)
                {
                    userStateToLifetime.Remove(asyncOp.UserSuppliedState);
                }
            }

            // Package the results of the operation in a 
            // CalculatePrimeCompletedEventArgs.
            MathMulCompletedEventArgs e = new MathMulCompletedEventArgs(mat1, mat2, mat_res, exception, canceled, asyncOp.UserSuppliedState);

            // End the task. The asyncOp object is responsible 
            // for marshaling the call.
            asyncOp.PostOperationCompleted(onCompletedDelegate, e);

            // Note that after the call to OperationCompleted, 
            // asyncOp is no longer usable, and any attempt to use it
            // will cause an exception to be thrown.
        }
        private bool TaskCancelled(object taskId)
        {
            return (tasks[taskId] == null);
        }

        /// <summary>
        /// Asynchoronous version of the method
        /// </summary>
        /// <param name="message">just simple message to display</param>
        /// <param name="userState">Unique value to maintain the task</param>
        /// 
        public void MathMulAsync(double[][] mat1, double[][] mat2, object userState)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(userState);
            //Multiple threads will access the task dictionary, so it must be locked to serialze access
            lock (tasks.SyncRoot)
            {
                if (tasks.Contains(userState))
                {
                    throw new ArgumentException("User state parameter must be unique", "userState");
                }
                tasks[userState] = asyncOp;
            }
            WorkerEventHandler worker = new WorkerEventHandler(MathMulWorker);
            //Execute process Asynchronously
            worker.BeginInvoke(mat1, mat2, asyncOp, null, null);
        }

        /// <summary>
        /// This method does the actual work
        /// </summary>
        /// <param name="message"></param>
        /// <param name="asyncOp"></param>
        private void MathMulWorker(double[][] mat1, double[][] mat2, AsyncOperation asyncOp)
        {
            double[][] mat_res = MatMultiply(mat1, mat2);

            lock (tasks.SyncRoot)
            {
                tasks.Remove(asyncOp.UserSuppliedState);
            }
            MathMulCompletedEventArgs e = new MathMulCompletedEventArgs(mat1, mat2, mat_res, null, false, asyncOp.UserSuppliedState);
            asyncOp.PostOperationCompleted(onCompletedDelegate, e);
        }

        public double getVal(double[][] mat, int row, int column)
        {
            return mat[row][column];
        }

        public static double[][] MatMultiply(double[][] mat1, double[][] mat2)
        {
            double[][] mat_res = new double[matSize][];
            for (int i = 0; i < matSize; i++)
            {
                mat_res[i] = new double[matSize];
            }
            for (int row = 0; row < mat1.Length; row++)
            {
                for (int col = 0; col < mat1[row].Length; col++)
                {
                    mat_res[row][col] = 0;
                    for (int i = 0; i < mat1.Length; i++)
                    {
                        mat_res[row][col] += mat1[row][i] * mat2[i][col];
                    }
                }
            }
            return mat_res;
        }


        public static double[][] CreateRandomMatrix()
        {
            double[][] mat = new double[matSize][];
            for (int i = 0; i < matSize; i++)
            {
                mat[i] = new double[matSize];
            }
            Random r = new Random();
            for (int i = 0; i < mat.Length; i++)
            {
                for (int j = 0; j < mat.Length; j++)
                {
                    mat[i][j] = r.Next(1, 10);
                }
            }
            return mat;
        }


        public static void show(double[][] mat)
        {
            for (int i = 0; i < mat.Length; i++)
            {
                for (int j = 0; j < mat.Length; j++)
                {
                    Console.Write(mat[i][j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        private static void onMathMulCompleted(object sender, MathMulCompletedEventArgs e)
        {
            lock (thisLock)
            {
                Console.WriteLine("macierz wejsciowa 1:");
                show(e.mat1);
                Console.WriteLine("macierz wejsciowa 2:");
                show(e.mat2);
                Console.WriteLine("macierz wynikowa: ");
                show(e.mat_res);
            }
        }
        static void Main(string[] args)
        {
            MathMul mathMul = new MathMul();
            mathMul.MathMulCompleted += onMathMulCompleted;
            for (int i = 0; i < 5; i++)
            {
                double[][] mat1 = CreateRandomMatrix();
                double[][] mat2 = CreateRandomMatrix();
                mathMul.MathMulAsync(mat1, mat2, i);
            }
            Console.ReadKey();
        }

    }



}