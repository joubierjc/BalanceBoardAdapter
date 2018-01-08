using BalanceBoardAdapter;
using System;
using System.Timers;

namespace Tests {
    class Program {
        static BalanceBoard board = null;

        static void Main(string[] args) {
            var infoUpdateTimer = new Timer() { Interval = 50, Enabled = false };
            infoUpdateTimer.Elapsed += infoUpdateTimer_Elapsed;

            Console.WriteLine("Connecting to a BalanceBoard");

            try {
                board = BalanceBoard.Create();
            }
            catch { }
            finally {
                if (board == null) {
                    Console.WriteLine("Could not connect to a BalanceBoard");
                }
            }

            Console.WriteLine("BalanceBoard Connected");

            infoUpdateTimer.Enabled = true;

            Console.ReadKey();

            infoUpdateTimer.Enabled = false;
            board.Disconnect();

        }

        static void infoUpdateTimer_Elapsed(object sender, ElapsedEventArgs e) {
            InfoUpdate();
        }

        static void InfoUpdate() {
            //var rwWeight = wiiDevice.WiimoteState.BalanceBoardState.WeightKg;

            //var rwTopLeft = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesKg.TopLeft;
            //var rwTopRight = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesKg.TopRight;
            //var rwBottomLeft = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesKg.BottomLeft;
            //var rwBottomRight = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesKg.BottomRight;

            var center = board.CenterOfGravity;

            //Console.WriteLine($"Weight: {rwWeight};");
            //Console.WriteLine($"TopLeft: {rwTopLeft}; TopRight: {rwTopRight}; BottomLeft: {rwBottomLeft}; BottomRight: {rwBottomRight};");
            Console.WriteLine($"Center of Gravity: X:{center.x}; Y:{center.y};");
        }
    }
}
