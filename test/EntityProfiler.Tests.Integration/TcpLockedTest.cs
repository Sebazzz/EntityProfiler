namespace EntityProfiler.Tests.Integration {
    using System.Diagnostics;
    using System.Threading;
    using NUnit.Framework;

    public abstract class TcpLockedTest {
        private Mutex _mutex;

        [SetUp]
        public void Lock() {
            Debug.WriteLine("Locking...");
            this._mutex = new Mutex(true, "TcpLock");
            this._mutex.WaitOne();
        }

        [TearDown]
        public void Unlock() {
            Debug.WriteLine("Unlocking...");
            this._mutex.ReleaseMutex();
            this._mutex.Close();
            this._mutex = null;
        }
    }
}