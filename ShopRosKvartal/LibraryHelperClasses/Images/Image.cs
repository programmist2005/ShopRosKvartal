using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.LibraryHelperClasses.Images
{
    public static class Image
    {
        //===========================================================================
        //Метод сохранения изображения на сервере
        public static string Save(HttpPostedFileBase file, string path, string name, string id)
        {
            if (file != null)
            {
                //если name не пустой
                if (name != null)
                {
                    //проверяем на сервере существование файла
                    string filePath = path + @"\" + name;
                    if (System.IO.File.Exists(filePath))
                    {
                        //если файл существует удаляем его
                        System.IO.File.Delete(filePath);
                        name = null;
                    }
                }
                //сохраняем файл с именем аналогичным id-пользователя и расширением загружаемого файла
                file.SaveAs(path + @"\" + id + Path.GetExtension(file.FileName));

                //получаем новое имя файла
                name = id + Path.GetExtension(file.FileName);

                //возвращаем имя файла
                return name;
            }
            return null;
        }
        //===========================================================================



        //===========================================================================
        //Метод удаления изображения с сервера
        public static string Delete(string path, string name)
        {
            //если name не пустой
            if (name != null)
            {
                //проверяем на сервере существование файла
                string filePath = path + @"\" + name;
                if (System.IO.File.Exists(filePath))
                {
                    //если файл существует удаляем его
                    System.IO.File.Delete(filePath);
                    name = null;
                }
            }
            //возвращаем имя файла
            return name;
        }
        //===========================================================================
    }
}