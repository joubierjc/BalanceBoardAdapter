using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using WiimoteLib;

namespace BalanceBoardAdapter {
    internal class DeviceDiscovery {
        /// <summary>
        /// Recherche et active tous les appareils Wii voulant se synchroniser.
        /// </summary>
        /// <param name="removeExisting">Défaut: true; Supprime les appareils Wii connus</param>
        public static void DiscoverWiiDevices(bool removeExisting = true) {
            using (var bluetoothClient = new BluetoothClient()) {

                // Par défaut, on souhaite supprimer, les appareils Wii de notre listes d'appareils bluetooth.
                // Sinon, on peut pas recommencer une sync avec une wiimote/board/etc..
                // Du coup on les cherches tous et on les deletes.
                if (removeExisting) {
                    // On cherche tous les appareils bluetooth connus.
                    var bluetoothExistingList = bluetoothClient.DiscoverDevices(255, false, true, false);
                    foreach (var item in bluetoothExistingList) {
                        // Si l'appareil ne contient pas Nintendo dans son nom, on le zappe.
                        if (!item.DeviceName.Contains("Nintendo")) continue;
                        // Si c'est un appareil Nintendo, on le delete.
                        BluetoothSecurity.RemoveDevice(item.DeviceAddress);
                        item.SetServiceState(BluetoothService.HumanInterfaceDevice, false);
                    }
                }

                // On cherche tous les appareils bluetooth inconnus.
                var bluetoothDiscoveredList = bluetoothClient.DiscoverDevices(255, false, false, true);
                // Une variable utilisée pour filtrer les appareils en cours de synchro, qui ne sont pas de Nintendo
                var bluetoothIgnored = 0;
                foreach (var item in bluetoothDiscoveredList) {
                    // Au cas où un appareil ne venant pas de la Wii serait en cours de synchro.
                    if (!item.DeviceName.Contains("Nintendo")) {
                        bluetoothIgnored += 1; // On augmente notre compteur et on ne fait rien avec l'appareil trouvé.
                        continue;
                    }

                    // SYNC PERMANTE ICI, SI JAMAIS ON EN A VRAIMENT BESOIN.

                    // On a trouvé un appareil venant de la Wii, on l'installe comme appareil HID, et on lui laisse un peu de temps pour finir tous ça.
                    item.SetServiceState(BluetoothService.HumanInterfaceDevice, true);
                }
                // Au cas où le pc est un peu long, on lui laisse le temps de finir l'installation avant de se connecter.
                // Valeur arbitraire.
                System.Threading.Thread.Sleep(5000);

                // On se connecte à chaque appareils une fois pour ne pas qu'ils partent en sommeil et disparaissent.
                if (bluetoothDiscoveredList.Length > bluetoothIgnored) {
                    // On cherche tous les appareils Wii.
                    var devices = new WiimoteCollection();
                    devices.FindAllWiimotes();

                    foreach (var wiiDevice in devices) {
                        // On se connecte, set une led allumée/éteinte, et se déconnecte.
                        wiiDevice.Connect();
                        wiiDevice.SetLEDs(true, false, false, false);
                        wiiDevice.SetLEDs(false, false, false, false);
                        wiiDevice.Disconnect();
                    }
                }
            }
        }
    }

    // UN MORCEAU DE CODE POUR LA SYNC PERMANENTE
    // CA MARCHAIT PAS SUR MA MACHINE
    // DONC JE ZAPPE, MAIS JE LE LAISSE COMMENTE SI JAMAIS ON ME METS TROP DE PRESSION
    // VIENT DE LA LIB QUI M'A INSPIRE

    //// Send special pin for permanent sync.
    //if (checkBox_PermanentSync.Checked) {
    //    // Sync button requires host address, holding 1+2 buttons requires device address.
    //    var btPin = AddressToWiiPin(BluetoothRadio.PrimaryRadio.LocalAddress.ToString());
    //    // Pin needs to be added before doing the pair request.
    //    new BluetoothWin32Authentication(btItem.DeviceAddress, btPin);
    //    // Null forces legacy pin request instead of SSP authentication.
    //    BluetoothSecurity.PairRequest(btItem.DeviceAddress, null);
    //}
}
