using System;
using System.Threading;

namespace SemaphoreDeadlock {
	class Program {
		static void Main() {

			using( ManualResetEvent hasSemaphoreEvent = new ManualResetEvent( false ) ) {

				using( SemaphoreSlim semaphore = new SemaphoreSlim( 1 ) ) {
					Console.WriteLine( $"Created semaphore [ CurrentCount: { semaphore.CurrentCount } ]" );

					Thread thread = new Thread( () => {
						try {
							Console.WriteLine( $"Waiting on semaphore [ CurrentCount: { semaphore.CurrentCount } ]" );
							semaphore.Wait();

							Console.WriteLine( $"Semaphore acquired [ CurrentCount: { semaphore.CurrentCount } ] " );
							hasSemaphoreEvent.Set();

							// Simulate case where try isn't entered in time
							Thread.Sleep( TimeSpan.FromSeconds( 5 ) );

							try {
								Console.WriteLine( $"Inside try [ CurrentCount: { semaphore.CurrentCount } ] " );

							} finally {
								Console.WriteLine( $"Releasing semaphore [ CurrentCount: { semaphore.CurrentCount } ]" );
								semaphore.Release();
							}

						} catch ( ThreadAbortException ) {
							Console.WriteLine( $"Thread aborted  [ CurrentCount: { semaphore.CurrentCount } ] " );
						}
					} );

					thread.Start();
					hasSemaphoreEvent.WaitOne();
					thread.Abort();
					thread.Join();

					Console.WriteLine( $"Thread completed  [ CurrentCount: { semaphore.CurrentCount } ] " );
					if( semaphore.CurrentCount == 0 ) {
						Console.WriteLine( "Semaphore deadlocked. Checkmate!!!" );
					}
				}
			}
		}
	}
}
