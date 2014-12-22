using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using OSProject.Model;
using OSProject.Model.Structures;
using Excel = Microsoft.Office.Interop.Excel;

namespace OSProject.Controller
{
    public class ExcelInteraction
    {
        
        public static async Task <ObservableCollection<ResultData>> OpenFile()
        {
            return await Task.Run(() =>
            {
                var dialog = new OpenFileDialog {Filter = "Excel файлы(*.xlsx)|*.xlsx", CheckFileExists = true};
                if (dialog.ShowDialog() == true)
                {
                    var data = new ObservableCollection<ResultData>();
                    var excelapp = new Excel.Application();
                    //Открываем книгу и получаем на нее ссылку
                    var excelappworkbook = excelapp.Workbooks.Open(dialog.FileName,
                     Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                     Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                     Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                     Type.Missing, Type.Missing);
                    //Получаем ссылку на лист 1
                    var excelworksheet = (Excel.Worksheet)excelappworkbook.Sheets[1];
                    var range = excelworksheet.UsedRange;

                    for (var i = 1; i <= range.Rows.Count; i++)
                    {
                        data.Add(new ResultData
                        {
                            Consumption = ((Excel.Range)range.Cells[i, 1]).Value,
                            LevelDeviation = ((Excel.Range)range.Cells[i, 2]).Value,
                            Pressure = ((Excel.Range)range.Cells[i, 3]).Value
                        });

                    }
                    excelappworkbook.Close(false, Type.Missing, Type.Missing);
                    excelapp.Quit();

                    ReleaseObject(excelworksheet);
                    ReleaseObject(excelappworkbook);
                    ReleaseObject(excelapp);
                    return data;
                }
                return null;
            });
        }

        public static async Task SaveFile(ObservableCollection<ResultData> data)
        {
            await Task.Run(() =>
            {
                try
                {
                    var eDialog = new SaveFileDialog
                    {
                        CheckFileExists = false,
                        OverwritePrompt = true,
                        Filter = "Excel файлы(*.xlsx)|*.xlsx"
                    };
                    if (eDialog.ShowDialog() == true)
                    {
                        var i = 1;
                        var excelapp = new Excel.Application();
                        //Открываем книгу и получаем на нее ссылку
                        var excelappworkbook = excelapp.Workbooks.Add(Type.Missing);
                        //Получаем ссылку на лист 1
                        var excelworksheet = (Excel.Worksheet) excelappworkbook.Sheets[1];
                        foreach (var resultData in data)
                        {
                            excelworksheet.Cells[i, 1] = resultData.Consumption;
                            excelworksheet.Cells[i, 2] = resultData.LevelDeviation;
                            excelworksheet.Cells[i, 3] = resultData.Pressure;
                            i++;
                        }
                        excelappworkbook.SaveAs(eDialog.FileName);
                        excelappworkbook.Close(true, Type.Missing, Type.Missing);
                        excelapp.Quit();

                        ReleaseObject(excelworksheet);
                        ReleaseObject(excelappworkbook);
                        ReleaseObject(excelapp);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            });
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex);
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
