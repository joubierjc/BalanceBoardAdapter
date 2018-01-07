using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WiimoteLib;

namespace BalanceBoardAdapter {
    public class BalanceBoard {
        private Wiimote wiiDevice;

        // Constructor
        private BalanceBoard(Wiimote wiiDevice) {
            this.wiiDevice = wiiDevice;
        }

        /// <summary>
        /// Crée une BalanceBoard, si il en existe une dans le réseau Bluetooth proche.
        /// </summary>
        /// <returns></returns>
        public static BalanceBoard Create() {
            try {
                return GetDevice();
            }
            catch (Exception ex) {
                // Si on n'a pas d'appareils dans la liste pour le moment, alors on essaie d'en synchro.
                // 4 essais, c'est arbitraire, on peut choisir autre chose.
                for (var i = 0; i < 4; i++) {
                    try {
                        // On lance une recherche Bluetooth et on ré-éssaie de connecter une board.
                        DeviceDiscovery.DiscoverWiiDevices();
                        return GetDevice();
                    }
                    catch { }
                }
                // Si on ne trouve toujours pas d'appareil Wii, on remonte l'erreur.
                throw ex;
            }
        }

        /// <summary>
        /// Renvoie une BalanceBoard
        /// </summary>
        /// <returns></returns>
        private static BalanceBoard GetDevice() {
            // On cherche tous les appareils Wii.
            var devices = new WiimoteCollection();
            devices.FindAllWiimotes();
            foreach (var device in devices) {
                // On vérifie que l'appareil est bien une board.
                device.Connect(); // On a besoin de connecter le device pour savoir quel est son ExtensionType.
                if (device.WiimoteState.ExtensionType == ExtensionType.BalanceBoard) {
                    var board = new BalanceBoard(device);
                    board.Connect();
                    return board;
                }
                device.Disconnect();
            }
            return null;
        }

        /// <summary>
        /// Connecte la BalanceBoard
        /// </summary>
        public void Connect() {
            wiiDevice.Connect();
            // Je sais pas si c'est vraiment utile, mais dans le doute je le met, vu que la lib sur laquelle je m'inspire le met aussi.
            wiiDevice.SetReportType(InputReport.IRAccel, false);
            SetLED(true);
        }

        /// <summary>
        /// Déconnecte laBalanceBoard
        /// </summary>
        public void Disconnect() {
            SetLED(false);
            wiiDevice.Disconnect();
        }

        /// <summary>
        /// Change la valeur de la LED de la BalanceBoard
        /// </summary>
        /// <param name="enabled"></param>
        public void SetLED(bool enabled) {
            wiiDevice.SetLEDs(enabled, false, false, false);
        }

        /// <summary>
        /// Renvoie le Centre de gravité
        /// </summary>
        /// <returns></returns>
        public Tuple<float, float> GetCenterOfGravity() {
            return new Tuple<float, float>(
                wiiDevice.WiimoteState.BalanceBoardState.CenterOfGravity.X,
                wiiDevice.WiimoteState.BalanceBoardState.CenterOfGravity.Y
            );
        }

        /// <summary>
        /// Renvoie le Centre de gravité
        /// </summary>
        /// <returns></returns>
        public void GetCenterOfGravity(out float x, out float y) {
            x = wiiDevice.WiimoteState.BalanceBoardState.CenterOfGravity.X;
            y = wiiDevice.WiimoteState.BalanceBoardState.CenterOfGravity.Y;
        }
    }
}
