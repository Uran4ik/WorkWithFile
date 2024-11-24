using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StrategyPatternWithGenerics
{
    public interface IWriteStartegy<T>
    { Task WriteToFileAsync(string path, T content); }
    public interface IReadStrategy<T>
    { Task ReadFromFileAsync(string path); }

    public class WriteToFileStrategy : IWriteStartegy<string>
    {
        public async Task WriteToFileAsync(string path, string content)
        {
            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(nameof(content), "Введи строку");

            using (StreamWriter streamWriter = new StreamWriter(path, append: true))
                await streamWriter.WriteLineAsync(content);
        }
    }

    public class ReadFromFileStrategy : IReadStrategy<string>
    {
        public async Task ReadFromFileAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Файл не найдены: {path}");

            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                string content = await reader.ReadToEndAsync();
                Console.WriteLine($"Контент файла: {content}");
            }
        }
    }
    public class FileOperationContext<T>
    {
        private IWriteStartegy<T> _writeStrategy;
        private IReadStrategy<T> _readStrategy;

        public void SetWriteStrategy(IWriteStartegy<T> strategy)
            => _writeStrategy = strategy;

        public void SetReadStrategy(IReadStrategy<T> strategy)
            => _readStrategy = strategy;

        public async Task WriteAsync(string path, T data)
        {
            if (_writeStrategy == null)
                throw new InvalidOperationException("Стратегия записи не установлена.");
            await _writeStrategy.WriteToFileAsync(path, data);
        }

        public async Task ReadAsync(string path)
        {
            if (_readStrategy == null)
                throw new InvalidOperationException("Стратегия чтения не установлена.");
            await _readStrategy.ReadFromFileAsync(path);
        }
    }
    public class GenericFileHandler<T>
    {
        private FileOperationContext<T> _context;

        public GenericFileHandler()
           => _context = new FileOperationContext<T>();

        public async Task WriteAsync(string path, T data)
        {
            _context.SetWriteStrategy(new WriteToFileStrategy() as IWriteStartegy<T>);
            await _context.WriteAsync(path, data);
        }

        public async Task ReadAsync(string path)
        {
            _context.SetReadStrategy(new ReadFromFileStrategy() as IReadStrategy<T>);
            await _context.ReadAsync(path);
        }
    }


    class Program
    {
        static async Task Main(string[] args)
        {
            var fileHandler = new GenericFileHandler<string>();

            Console.WriteLine("1 - Вписать в файл строку, 2 - Прочитать файл");
            string choice = Console.ReadLine();

            Console.Write("Напишите путь: ");
            string path = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Напишите текст: ");
                await fileHandler.WriteAsync(path, Console.ReadLine());
            }
            else if (choice == "2")
                await fileHandler.ReadAsync(path);
            Console.ReadKey();
        }
    }
}