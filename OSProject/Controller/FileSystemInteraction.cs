using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using OSProject.Model;
using OSProject.Model.Structures;

namespace OSProject.Controller
{
    /// <summary>
    /// Класс FileSystemInteraction
    /// 
    /// Отвечает за взаимодействие программы с файловой системой. Содержит функции для сохранения и открытия файлов.
    /// Помогает с отслеживание необходимости сохранения файла
    /// </summary>
    public class FileSystemInteraction
    {
        private static string _currentlyOpenedFile = "Untitled.mml";                             // Открытый в данный момент файл

        /// <summary>
        /// Функция SaveFile(InputData inpDObj)
        /// 
        /// Сохраняет объект в файл. Введенные пользователем данные, сформированные в объект
        /// сохраняются в JSON формате
        /// </summary>
        /// <param name="inpDObj"></param>
        public async static Task SaveFile(ObservableCollection<ResultData> inpDObj)
        {
            if (inpDObj == null) return;                                                        // Если поступил null объект, выходим из функции
            if (_currentlyOpenedFile != "Untitled.mml")                                         // Если был открыт файл, перезаписываем в него
            {
                await OutputObjectToFile(inpDObj, _currentlyOpenedFile);                        // Выводим объект в этот файл
            }
            else                                                                                // Если нет, то
            {
                await SaveAsFile(inpDObj);                                                      // Действуем по аналогии с SaveAs
            }
        }

        /// <summary>
        /// Функция SaveFile(InputData inpDObj)
        /// 
        /// Сохраняет объект в файл, предложив окно сохранения. Введенные пользователем данные, сформированные в объект
        /// сохраняются в JSON формате
        /// </summary>
        /// <param name="inpDObj"></param>
        public async static Task SaveAsFile(ObservableCollection<ResultData> inpDObj)
        {
            var sdg = new Microsoft.Win32.SaveFileDialog                                    // Создаем диалоговое окно сохранения 
            {
                FileName = "Document",                                                      // Имя файла по умолчанию
                DefaultExt = ".mml",                                                        // Расширение файла по умолчанию
                Filter = "MM Data (.mml)|*.mml"                                             // Фильтр расширений файла
            };
            var result = sdg.ShowDialog();                                                  // Открывает диалоговое окно сохранения
            if (result != true) return;                                                     // Обрабатываем результат выполнения диалогового окна сохранения
            var filename = sdg.FileName;                                                    // Получаем имя файла, введенное пользователем
            await OutputObjectToFile(inpDObj, filename);                                    // Записываем объект в файл
            _currentlyOpenedFile = filename;                                                // Сохраняем имя файла
        }
        /// <summary>
        /// Функция InputData OpenFile()
        /// 
        /// Открывает диалоговое окно открытия файла, получает от пользователя файл и формирует объект полученный из файла
        /// Затем возвращает файл в виде объекта.
        /// 
        /// Функция отвечает за проверку сохранности открытых файлов.
        /// </summary>
        /// <returns></returns>
        public async static Task<ObservableCollection<ResultData>> OpenFile()
        {
            ObservableCollection<ResultData> inpDObj = null;                                                      // Описываем возвращаемый объект
            if (_currentlyOpenedFile == "Untitled.mml")                                    // Если файл еще не был открыт или сохранен
            {
                inpDObj = await openFileDialogHandler();                                   // Вызываем функцию получения объекта
            }
            else                                                                           // Если нет, то 
            {

                if (MessageBox.Show(                                                // Спрашиваем пользователя, действительно ли
                                                                                  // он хочет открыть файл, возможно потеряв
                    "Вы действительно хотите открыть " +                                  // данные из старого файла
                    "другой файл? Предыдущие данные будут утеряны",
                    "Предупреждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question)
                    == MessageBoxResult.Yes)                                             // Если да, то
                {
                    inpDObj = await openFileDialogHandler();                             // Вызываем функцию получения объекта
                }
            }

            return inpDObj;                                                              // Возвращаем объект из файла
        }

        /// <summary>
        /// Функция InputData openFileDialogHandler()
        /// 
        /// Открывает диалоговое окно открытия файла, получает от пользователя файл и формирует объект полученный из файла
        /// Затем возвращает файл в виде объекта. 
        /// 
        /// Функция отвечает за получение объекта
        /// </summary>
        /// <returns></returns>
        private async static Task<ObservableCollection<ResultData>> openFileDialogHandler()
        {
            var opfd = new Microsoft.Win32.OpenFileDialog                               // Создаем диалоговое окно открытия
            {
                FileName = "Document",                                                  // Имя файла по умолчанию 
                DefaultExt = ".mml",                                                    // Расширение файла по умолчанию
                Filter = "MM Data (.mml)|*.mml"                                         // Фильтр расширений файла
            };

            var result = opfd.ShowDialog();                                             // Открывает диалоговое окно открытия
            if (result != true) return null;                                            // Обрабатываем результат выполнения диалогового окна открытия
            var filename = opfd.FileName;                                               // Получаем имя файла, выбранного пользователем
            var inpDObj =                                                               // Десереализуем объект 
                await InputObjectFromFile(filename);                                    // полученный из файла
            _currentlyOpenedFile = filename;                                            // Сохраняем имя открытого файла
            return inpDObj;                                                             // Возвращаем объект
        }

        /// <summary>
        /// Функция OutputObjectToFile(InputData inDObj, string filename)
        /// 
        /// Функция, выводящая объект в заданный файл
        /// </summary>
        /// <param name="inDObj"></param>
        /// <param name="filename"></param>
        private async static Task OutputObjectToFile(ObservableCollection<ResultData> inDObj, string filename)
        {
            if (File.Exists(filename)) File.Delete(filename);                         // Если файл существует, удаляем его
            var json = JsonConvert.SerializeObject(inDObj);                           // Сериализуем объект в строку JSON
            using (var swt = new StreamWriter(new FileStream(filename,                // Вывод в файл
                FileMode.OpenOrCreate, FileAccess.Write)))
            {
                await swt.WriteAsync(json);                                           // Записываем строку в файл
                swt.Close();                                                          // Закрываем поток записи
            }
        }
        /// <summary>
        /// Функция InputObjectFromFile(string filename)
        /// 
        /// Функция получающая строку JSON из файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private async static Task<ObservableCollection<ResultData>> InputObjectFromFile(string filename)
        {
            string json;                                                             // Строка JSON
            using (var srd = new StreamReader(new FileStream(filename,               // Вывод в файл
                FileMode.Open, FileAccess.Read)))
            {
                json = await srd.ReadToEndAsync();                                   // Считываем весь файл в строку
                srd.Close();                                                         // Закрываем поток считывания
            }
            return JsonConvert.DeserializeObject<ObservableCollection<ResultData>>(json);                   // Возвращаем строку с данными

        }
    }
}
