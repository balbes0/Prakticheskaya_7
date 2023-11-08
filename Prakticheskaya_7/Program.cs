using System.Diagnostics;

class Program
{
    static void Main()
    {
        Console.CursorVisible = false;
        ConsoleKeyInfo key;
        DriveInfo[] drives = DriveInfo.GetDrives();
        int currentDriveIndex = 0;

        do
        {
            Console.Clear();
            DisplayDriveMenu(drives, currentDriveIndex);

            key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    currentDriveIndex = (currentDriveIndex - 1 + drives.Length) % drives.Length;
                    break;
                case ConsoleKey.DownArrow:
                    currentDriveIndex = (currentDriveIndex + 1) % drives.Length;
                    break;
                case ConsoleKey.Enter:
                    ExploreDrive(drives[currentDriveIndex].RootDirectory);
                    break;
            }

        } while (key.Key != ConsoleKey.Escape);
    }

    static void DisplayDriveMenu(DriveInfo[] drives, int currentDriveIndex)
    {
        Console.WriteLine("                                          ВАШ КОМПЬЮТЕР");
        Console.WriteLine("------------------------------------------------------------------------------------------------");
        Console.WriteLine("Выберите диск:");
        Console.WriteLine();
        for (int i = 0; i < drives.Length; i++)
        {
            Console.Write(i == currentDriveIndex ? "> " : "  ");
            Console.WriteLine($"{drives[i].Name} ({FormatBytes(drives[i].TotalFreeSpace)} свободно из {FormatBytes(drives[i].TotalSize)})");
        }

        Console.WriteLine("\nНажмите Enter для выбора диска, Escape для выхода.");
    }

    static void ExploreDrive(DirectoryInfo directory)
    {
        ConsoleKeyInfo key;
        FileSystemInfo[] items = directory.GetFileSystemInfos();
        int currentItemIndex = 0;

        do
        {
            Console.Clear();
            DisplayDirectoryContent(items, currentItemIndex);

            key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    currentItemIndex = (currentItemIndex - 1 + items.Length) % items.Length;
                    break;
                case ConsoleKey.DownArrow:
                    currentItemIndex = (currentItemIndex + 1) % items.Length;
                    break;
                case ConsoleKey.Enter:
                    if (items[currentItemIndex] is DirectoryInfo)
                    {
                        ExploreDrive((DirectoryInfo)items[currentItemIndex]);
                    }
                    else if (items[currentItemIndex] is FileInfo)
                    {
                        ExecuteFile((FileInfo)items[currentItemIndex]);
                    }
                    break;
                case ConsoleKey.F1:
                    CreateFolder(directory);
                    items = directory.GetFileSystemInfos();
                    break;
                case ConsoleKey.F2:
                    CreateFile(directory);
                    items = directory.GetFileSystemInfos();
                    break;
                case ConsoleKey.F3:
                    DeleteFile(items[currentItemIndex]);
                    items = directory.GetFileSystemInfos();
                    break;
            }

        } while (key.Key != ConsoleKey.Backspace);
    }

    static void DeleteFile(FileSystemInfo fileSystemInfo)
    {
        try
        {
            if (fileSystemInfo is FileInfo)
            {
                File.Delete(fileSystemInfo.FullName);
                Console.WriteLine($"Файл \"{fileSystemInfo.Name}\" успешно удален!");
            }
            else if (fileSystemInfo is DirectoryInfo)
            {
                Directory.Delete(fileSystemInfo.FullName, true);
                Console.WriteLine($"Папка \"{fileSystemInfo.Name}\" успешно удалена!");
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
        catch
        {
            Console.WriteLine($"Ошибка при удалении файла/папки!");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }


    static void CreateFile(DirectoryInfo parentDirectory)
    {
        Console.Clear();
        Console.Write("Введите имя нового файла: ");
        string fileName = Console.ReadLine();

        try
        {
            string newFilePath = Path.Combine(parentDirectory.FullName, fileName);
            File.Create(newFilePath).Close();
            Console.WriteLine($"Файл \"{fileName}\" успешно создан!");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
        catch
        {
            Console.WriteLine($"Ошибка при создании файла!");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

    static void CreateFolder(DirectoryInfo parentDirectory)
    {
        Console.Clear();
        Console.Write("Введите имя новой папки: ");
        string folderName = Console.ReadLine();

        try
        {
            string newFolderPath = Path.Combine(parentDirectory.FullName, folderName);
            Directory.CreateDirectory(newFolderPath);
            Console.WriteLine($"Папка \"{folderName}\" успешно создана!");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
        catch
        {
            Console.WriteLine($"Ошибка при создании папки!");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }


    static void DisplayDirectoryContent(FileSystemInfo[] items, int currentItemIndex)
    {
        Console.WriteLine("  СОДЕРЖИМОЕ ДИСКА     F1 - ДЛЯ СОЗДАНИЯ ПАПКИ     F2 - ДЛЯ СОЗДАНИЯ ФАЙЛА     F3 - УДАЛЕНИЯ ФАЙЛА/ПАПКИ");
        Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("  Папки/файлы:                    Дата создания:                    Тип файла:");
        Console.WriteLine();

        for (int i = 0; i < items.Length; i++)
        {
            string itemName = items[i].Name;
            string creationTime = items[i].CreationTime.ToString("yyyy-MM-dd HH:mm:ss");

            string fileType = (items[i] is DirectoryInfo) ? "Папка" : $"{Path.GetExtension(itemName)}";

            Console.Write(i == currentItemIndex ? "> " : "  ");
            Console.WriteLine($"{itemName,-30} {creationTime,20} {fileType,19}");
        }

        Console.WriteLine("\nНажмите Enter для выбора элемента, Backspace для возврата.");
    }

    static void ExecuteFile(FileInfo file)
    {
        Console.WriteLine($"Открывается файл: {file.FullName}");

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = file.FullName,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }
        catch
        {
            Console.WriteLine($"Ошибка при открытии файла!");
        }

        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int suffixIndex = 0;

        while (bytes >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            bytes /= 1024;
            suffixIndex++;
        }

        return $"{bytes} {suffixes[suffixIndex]}";
    }
}