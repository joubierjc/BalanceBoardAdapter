using System;
using WiimoteLib;

namespace BalanceBoardAdapter {
    public struct VectorF {
        public float x;
        public float y;
    }

    public struct SensorsF {
        public float TopLeft;
        public float BottomLeft;
        public float TopRight;
        public float BottomRight;
    }

    public struct Sensors {
        public short TopLeft;
        public short BottomLeft;
        public short TopRight;
        public short BottomRight;
    }

    public class BalanceBoard {
        private Wiimote wiiDevice;

        /// <summary>
        /// Renvoie les valeurs des senseurs en Kg.
        /// </summary>
        public SensorsF SensorValuesKg {
            get {
                return new SensorsF() {
                    TopLeft = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesKg.TopLeft,
                    BottomLeft = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesKg.BottomLeft,
                    TopRight = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesKg.TopRight,
                    BottomRight = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesKg.BottomRight
                };
            }
        }

        /// <summary>
        /// Renvoie la masse en Kg.
        /// </summary>
        public float WeightKg {
            get {
                return wiiDevice.WiimoteState.BalanceBoardState.WeightKg;
            }
        }

        /// <summary>
        /// Renvoie les valeurs des senseurs en Lb.
        /// </summary>
        public SensorsF SensorValuesLb {
            get {
                return new SensorsF() {
                    TopLeft = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesLb.TopLeft,
                    BottomLeft = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesLb.BottomLeft,
                    TopRight = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesLb.TopRight,
                    BottomRight = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesLb.BottomRight
                };
            }
        }

        /// <summary>
        /// Renvoie la masse en Lb.
        /// </summary>
        public float WeightLb {
            get {
                return wiiDevice.WiimoteState.BalanceBoardState.WeightLb;
            }
        }

        /// <summary>
        /// Renvoie les valeurs des senseurs, sans modification.
        /// </summary>
        public Sensors SensorValuesRaw {
            get {
                return new Sensors() {
                    TopLeft = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesRaw.TopLeft,
                    BottomLeft = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesRaw.BottomLeft,
                    TopRight = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesRaw.TopRight,
                    BottomRight = wiiDevice.WiimoteState.BalanceBoardState.SensorValuesRaw.BottomRight
                };
            }
        }

        /// <summary>
        /// Renvoie le Centre de gravité.
        /// </summary>
        public VectorF CenterOfGravity {
            get {
                return new VectorF() {
                    x = wiiDevice.WiimoteState.BalanceBoardState.CenterOfGravity.X,
                    y = wiiDevice.WiimoteState.BalanceBoardState.CenterOfGravity.Y
                };
            }
        }

        /// <summary>
        /// Construit une balance board à partir d'un appareil Wii.
        /// </summary>
        /// <param name="wiiDevice"></param>
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
    }
}
