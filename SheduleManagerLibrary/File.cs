using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.IO;

namespace SheduleManagerLibrary
{
    public class File
    {
        /// <summary>
        /// Загузка данных из файла
        /// </summary>
        /// <param name="fileName">Путь с именем файла</param>
        /// <param name="fileData">Содержимое файла (возвращаемый параметр)</param>
        /// <param name="result">Если результат выполнения успешен, то возвращает null, иначе текст ошибки</param>
        /// <returns></returns>
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static void Load(SqlString fileName, out SqlBinary fileData, out SqlString result)
        {
            result = new SqlString();//SqlString.Null!!!
            fileData = new SqlBinary();
            try
            {
                if (fileName.IsNull)
                {
                    result = "Параметр <FileName> не может быть пустым (null)";
                    return;
                }
                if (!System.IO.File.Exists(fileName.Value))
                {
                    result = string.Format("Файл <{0}> не существует, или недостаточно прав для доступа к файлу", fileName.Value);
                    return;
                }
                try
                {
                    using (var fileStream = new System.IO.FileStream(fileName.Value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        int length = (int)fileStream.Length;
                        var buffer = new byte[length];
                        int count;
                        int sum = 0;
                        while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                            sum += count;  // sum is a buffer offset for next reading
                        fileData = new SqlBinary(buffer);
                    }
                }
                catch (Exception e)
                {
                    result = e.Message;
                    return;
                }
                return;
            }
            catch (Exception e)
            {
                result = e.Message;
            }
        }
    }
}
