using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLayer
{
    public class Logger
    {
        private static string logFilePath = "C:\\Users\\David\\Desktop\\Universidad\\Septimo Semestre\\Distribuidas\\Unidad 1\\Proyecto U1\\appLog.txt"; // Define el path donde se guardará el archivo de log

        // Método para registrar mensajes en el log
        public static void LogMessage(string message)
        {
            try
            {
                // Verifica si el directorio de logs existe, si no, lo crea
                if (!Directory.Exists(Path.GetDirectoryName(logFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
                }

                // Escribe el mensaje con la fecha y hora actual
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {message}");
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error al escribir en el log, escribe el error en otro lugar
                Console.WriteLine($"Error al escribir en el log: {ex.Message}");
            }
        }
    }
}
