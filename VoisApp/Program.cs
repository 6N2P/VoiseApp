using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WindowsInput;
using WindowsInput.Native;
using NAudio.Wave;
using Newtonsoft.Json;
using Vosk;
using System.Diagnostics;

namespace VoisApp
{

    internal class Program
    {
        private static WaveInEvent audioWaveMicrophone;
        private static VoskRecognizer voiceRecognizer;
        private static IMouseSimulator mouseSimulator;
        private static IKeyboardSimulator keyboardSimulator;
        private static Process process;

        private  Znachenie znachenie = new Znachenie();


        static void Main(string[] args)
        {
            Vosk.Vosk.SetLogLevel(-1);
            Vosk.Vosk.GpuThreadInit();

            //речевая модель
            voiceRecognizer = new VoskRecognizer(new Model(@"H:\\MyProgram\\vosk-model-small-ru-0.22"), 16000);

            audioWaveMicrophone = new WaveInEvent();
            audioWaveMicrophone.WaveFormat = new WaveFormat(16000, 1);
            audioWaveMicrophone.DataAvailable += AudioWaveMicrophone_DataAvailable;
            audioWaveMicrophone.BufferMilliseconds = 20;
            audioWaveMicrophone.StartRecording();

            mouseSimulator = new InputSimulator().Mouse;
            keyboardSimulator = new InputSimulator().Keyboard;

            Znachenie znachenie = new Znachenie();

            Console.ReadLine();

        }

        private static void AudioWaveMicrophone_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (voiceRecognizer.AcceptWaveform(e.Buffer, e.Buffer.Length))
            {
                string result = voiceRecognizer.Result();
                RecognitionAction(result);
            }
            else
            {
                string result = voiceRecognizer.PartialResult();
                RecognitionAction(result);
            }
        }
        private static void RecognitionAction(string text)
        {
            Result result = JsonConvert.DeserializeObject<Result>(text);
            if (!string.IsNullOrEmpty(result.partial)) // Часть сказанного
            {
                Console.WriteLine(result.partial);
                switch (result.partial)
                {
                    case string s when s.Contains("открой"):
                        
                        break;
                    case string s when s.Contains("выключи"):
                        break;
                }
            }
            if (!string.IsNullOrEmpty(result.text)) // Полностью сказанное
            {
                Console.Clear();
                Console.WriteLine(result.text);
                Znachenie znachenie = new Znachenie();
                
                 znachenie.znach = 2000;

                if (result.text.Contains("значение тысяча"))
                {
                    znachenie.znach = 2000;
                }

                if (result.text.Contains("открой браузер"))
                {
                    string target = "https://www.google.com/search?q=google";
                   process = System.Diagnostics.Process.Start(target);
                }
                if (result.text.Contains("вниз"))
                {
                    mouseSimulator.VerticalScroll(-znachenie.znach);
                    mouseSimulator.VerticalScroll(-znachenie.znach);
                }

                if (result.text.Contains("верх"))
                {
                    mouseSimulator.VerticalScroll(znachenie.znach);
                    mouseSimulator.VerticalScroll(znachenie.znach);
                }
                if (result.text.Contains("запусти эксель"))
                {
                    string target = @"C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\Excel 2016";
                    process = System.Diagnostics.Process.Start(target);
                }
                if (result.text.Contains("запусти компас"))
                {
                    string target = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\КОМПАС-3D v20 x64\КОМПАС-3D v20";
                    process = System.Diagnostics.Process.Start(target);
                }
                if (result.text.Contains("открой ютуб"))
                {
                    string target = "https://www.youtube.com/";
                    process = System.Diagnostics.Process.Start(target);
                }
                if (result.text.Contains("открой папку загрузки"))
                {
                    string target =@"C:\\Users\\user\\Downloads";
                    process = System.Diagnostics.Process.Start(target);
                }
                if (result.text.Contains("открой сайт иконок"))
                {
                    string target = "https://icons8.ru/icons";
                    process = System.Diagnostics.Process.Start(target);
                }
                if (result.text.Contains("включи радио панк"))
                {
                    string target = "https://maximum.ru/online/punk";
                    process = System.Diagnostics.Process.Start(target);
                }
                if (result.text.Contains("запусти диспетчер задач"))
                {
                     keyboardSimulator.ModifiedKeyStroke(new[] {VirtualKeyCode.CONTROL, VirtualKeyCode.SHIFT},VirtualKeyCode.ESCAPE);                   
                }

            }
        }
    }
    public class Result // название переменных менять нельзя
    {
        public string partial;
        public string text;
    }

    public class Znachenie
    {
        public int znach;
    }
}
