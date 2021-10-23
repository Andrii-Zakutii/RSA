using Encryption.Core;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Key = Encryption.Core.Key;

namespace Encryption.Desktop.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private RSA _rsa = new RSA();
        private KeysGenerator _keysGenerator = new KeysGenerator();
        private string _filePath;
        private long _milliseconds;
        private KeyPair _keyPair;

        public MainViewModel()
        {
            _keyPair = _keysGenerator.GetRandomKeyPair();

            EncryptCommand = new Command(_ => EncodeFile(_keyPair.PublicKey, "Enciphered", "-enc-rsa", _rsa.DecryptBlock));

            DecryptCommand = new Command(_ => EncodeFile(_keyPair.PrivateKey, "Deciphered", "-dec-rsa", _rsa.DecryptBlock));

            ImportKeysCommand = new Command(_ =>
            {
                try
                {
                    if (!TryOpenFile("Import RSA keys", out var rsaKeysPath))
                        return;

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
                    saveFileDlg.FileName = "RSA_Keys.xml";
                    var dlgRes = saveFileDlg.ShowDialog();

                    if (dlgRes == true)
                    {
                        //File.WriteAllText(saveFileDlg.FileName, );
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

            SwapKeysCommand = new Command(_ =>
            {
                try
                {
                    if (_keyPair == null)
                        throw new ArgumentNullException("Key pair is not generated");

                    var buffer = _keyPair.PrivateKey;
                    _keyPair.PrivateKey = _keyPair.PublicKey;
                    _keyPair.PublicKey = buffer;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
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


        private void EncodeFile(Key key, string message, string padding, Func<byte[], Key, byte[]> encodingFunction)
        {
            if (File.Exists(_filePath) == false)
            {
                MessageBox.Show("File not exist!", "RSA");
                return;
            }

            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var encodedFileContent = encodingFunction(File.ReadAllBytes(_filePath), key);

                stopWatch.Stop();
                Milliseconds = stopWatch.ElapsedMilliseconds;

                File.WriteAllBytes(PaddFilename(_filePath, padding), encodedFileContent);
                MessageBox.Show(message, "RSA");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "RSA");
            }
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
