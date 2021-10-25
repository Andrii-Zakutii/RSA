using Encryption.Core;
using Encryption.Core.EncryptionServices;
using Encryption.Core.KeyGenerationServices;
using Encryption.Desktop.Models;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Key = Encryption.Core.Key;

namespace Encryption.Desktop.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private readonly ICryptoService _cryptoService = new RSACryptoService();
        private string _filePath;
        private long _milliseconds;
        private KeyPair _keyPair;

        public MainViewModel()
        {
            var keysGenerator = new CustomKeysGenerator();
            _keyPair = keysGenerator.CreateKeyPair();

            EncryptCommand = new Command(_ => ProcessFile(_keyPair.PublicKey, "-enc-rsa", CryptographicFunctionEnum.Encryption));

            DecryptCommand = new Command(_ => ProcessFile(_keyPair.PrivateKey, "-dec-rsa", CryptographicFunctionEnum.Decryption));

            ImportKeysCommand = new Command(_ =>
            {
                try
                {
                    if (!TryOpenFile("Import RSA keys", out var rsaKeysPath))
                        return;

                    _keyPair = (KeyPair)JsonSerializer.Deserialize(File.ReadAllText(rsaKeysPath), typeof(KeyPair));
                    MessageBox.Show($"Successfully imported RSA keys from '{rsaKeysPath}'");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    MessageBox.Show("Import RSA keys failed", "ERROR");
                }
            });

            ExportKeysCommand = new Command(_ =>
            {
                try
                {
                    var saveFileDlg = new SaveFileDialog();
                    saveFileDlg.Title = "Export RSA keys";
                    saveFileDlg.FileName = "RSA_Keys.json";
                    var dlgRes = saveFileDlg.ShowDialog();

                    if (dlgRes == true)
                    {
                        File.WriteAllText(saveFileDlg.FileName, JsonSerializer.Serialize(_keyPair));
                        MessageBox.Show($"File '{saveFileDlg.FileName}' saved.", "INFO");
                    }
                    else
                    {
                        MessageBox.Show($"File save canceled.", "WARNING");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    MessageBox.Show("Export RSA keys failed", "ERROR");
                }
            });

            SwapKeysCommand = new Command(_ => _keyPair = new KeyPair(_keyPair.PrivateKey, _keyPair.PublicKey));
        }

        public ICommand EncryptCommand { get; set; }

        public ICommand DecryptCommand { get; set; }

        public ICommand ExportKeysCommand { get; set; }

        public ICommand ImportKeysCommand { get; set; }

        public ICommand SwapKeysCommand { get; set; }

        public long Milliseconds
        {
            get => _milliseconds;

            set
            {
                _milliseconds = value;
                NotifyPropertyChanged(nameof(Milliseconds));
            }
        }

        public string FilePath
        {
            get => _filePath;

            internal set
            {
                _filePath = value;
                NotifyPropertyChanged(nameof(FilePath));
            }
        }

        private void ProcessFile(Key key, string padding, CryptographicFunctionEnum functionType)
        {
            if (File.Exists(_filePath) == false)
            {
                MessageBox.Show("File not exist!", "RSA");
                return;
            }

            try
            {
                var message = new Message(File.ReadAllBytes(_filePath));
                var processFunction = GetProcessFunction(message, functionType);
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                var processedFileContent = processFunction(_cryptoService, key);
                stopWatch.Stop();
                Milliseconds = stopWatch.ElapsedMilliseconds;
                File.WriteAllBytes(PaddFilename(_filePath, padding), processedFileContent.Content);
                MessageBox.Show("Done", "RSA");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "RSA");
            }
        }

        private Func<ICryptoService, Key, Message> GetProcessFunction(Message message, CryptographicFunctionEnum type)
        {
            return type == CryptographicFunctionEnum.Encryption ? message.Encrypt : message.Decrypt;
        }

        private bool TryOpenFile(string dlgTitle, out string filepath)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath1 = openFileDialog.FileName;
                filepath = filePath1;
                return true;
            }
            else
            {
                filepath = null;
                return false;
            }
        }

        private static string PaddFilename(string filePath, string padding)
        {
            var fi = new FileInfo(filePath);
            var fn = Path.GetFileNameWithoutExtension(filePath);
            return Path.Combine(fi.DirectoryName, fn + padding + fi.Extension);
        }
    }
}
