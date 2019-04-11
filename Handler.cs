using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;
using System.Threading;  
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NMS {

  public class DeviceSettings {
    public class Connection {
        [JsonProperty("portOut")]
        public int portOut { get; set;}
        [JsonProperty("label")]
        public int label { get; set;}
    }
    
    [JsonProperty("name")]
    public String name { get; set; }
    [JsonProperty("port")]
    public int port { get; set; }
    [JsonProperty("changed")]
    public bool changed { get; set; }
    [JsonProperty("connections")]
    public Connection[] connections { get; set;}
    [JsonProperty("active")]
    public bool active { get; set;}
}

  class Handler {
    Socket socket;
    Byte[] bytes;
    String data;
    String name;
    
    public Handler(Socket s) {
      this.socket = s;
      this.bytes = new Byte[1024];
      start();
    }

    private void start() {
      Thread t= new Thread(new ThreadStart(run));
      t.Start();
    }

    public static DeviceSettings[] checkConfigFile() {
        String dir = Directory.GetCurrentDirectory() + "/config.json";
        String st = File.ReadAllText(dir);
        DeviceSettings[] ds = JsonConvert.DeserializeObject<DeviceSettings[]>(st);
        return ds;
    }

    public static void updateConfigFile(DeviceSettings[] ds) {
        string json = JsonConvert.SerializeObject(ds, Formatting.Indented);
        File.WriteAllText(Directory.GetCurrentDirectory() + "/config.json", json);
    }

    public static void updateDevice(DeviceSettings ds) {

    }
    private void setDeviceActiveState(string name, bool state) {
      DeviceSettings[] ds = checkConfigFile();
        for (int i=0; i<ds.Length; i++) {
            if (ds[i].name == name && ds[i].active == false && state == true) {
                ds[i].active = state;
                Console.WriteLine("NMS: Changed device {0} to active", device_name);
                updateConfigFile(ds);
                return;
            } else if (ds[i].name == name && ds[i].active == true && state == false) {
                ds[i].active = state;
                Console.WriteLine("NMS: Changed device {0} to inactive", device_name);
                updateConfigFile(ds);
                return;
            }
        }
    }

    private void run() { 

      while(true) {

        while(true) {
          int byteREc = socket.Receive(bytes);
          data = Encoding.ASCII.GetString(bytes, 0, byteREc);
          Console.WriteLine(data);
          if (data.IndexOf("<keep_alive") > -1) {
            data = data.Replace("<keep_alive>", "");
            Console.WriteLine("NMS: Keep Alive - Device {0}", data);
            setDeviceActiveState(name, true);
            break;
          }
        }

        // try {
        //   DeviceSettings[] device_settings = checkConfigFile();

        //   int num_of_devices = device_settings.Length;
        //   for (int i = 0; i < num_of_devices; i++) {
        //       if (device_settings[i].changed == true && device_settings[i].active == true) {
        //           Console.WriteLine("NMS: " + device_settings[i].name + " settings changed");
        //           updateDevice(device_settings[i]);
        //           device_settings[i].changed = false;
        //           string new_connections = JsonConvert.SerializeObject(device_settings[i].connections, Formatting.Indented);
        //       }  
        //   }
        // } catch {

        // } 
      }
    }
  }

}