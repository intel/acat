using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PressagioAppModes
{
    public partial class FormApp : Form
    {
        bool pipeCreated;
        NamedPipeServer namedPipe;
        public static Label labelStatus;
        public FormApp()
        {
            InitializeComponent();
            Init();
            connectionStatus();
            labelStatus = label3;
        }
        string predictedWords;
        private void button1_Click(object sender, EventArgs e)
        {
            string strTextBox = textBox1.Text;
            if (namedPipe.clientConected)
                predictedWords = sendMessage(strTextBox);
            List<string> words = predictedWords.Split(',').ToList<string>();
            textBox2.Text = String.Join(Environment.NewLine, words);
            Debug.WriteLine(predictedWords);
            Debug.WriteLine("Last");
            Thread.Sleep(300);
            
        }

        public void Init()
        {

            namedPipe = new NamedPipeServer(Globals.PipeServerName, PipeDirection.InOut);
            pipeCreated = namedPipe.createPipeServer();
            if(!pipeCreated)
                pipeCreated = namedPipe.createPipeServer();
        }

        public string sendMessage(string text)
        {
            //Non Blocking
            //var task = Task.Run(async () => await option1());
            //var test = task.Result;
            //Non Blocking
            PressagioSetParam param = new PressagioSetParam("database type", "C:\\Users\abpulido\\OneDrive - Intel Corporation\\Desktop\\C# Projects\acat-psm2\\src\\Applications\\ACATApp\\bin\\Debug");
            string parms = JsonConvert.SerializeObject(param);
            PressagioMessage message = new PressagioMessage(PressagioMessage.PressagioMessageTypes.NextWordPredictionRequest, PressagioMessage.PressagioPredictionTypes.Normal, text);
            string jsonMessage = JsonConvert.SerializeObject(message);
            var code = Task.Run(() => option1(jsonMessage));
            //var code = Task.Run(() => option1(text));
            var test = code.Result;
            //Non Blocking
            //var test = option1();
            //Debug.WriteLine(test.Result);
            //Blocking
            /*Task<string> resultThingTask = Task.Run(() => option1());
            resultThingTask.Wait();
            var result = resultThingTask.Result;*/
            return test;
        }




        public async Task<string> option1(string text)
        {
            Debug.WriteLine("1");
            var test = await namedPipe.Write(text, 150);

            var answer = JsonConvert.DeserializeObject<WordAndCharacterPredictionResponse>(test);


            Debug.WriteLine("2");
            return test;
        }

        //Form1.labelStatus.Text = "Client conected";
        private async Task waitToFinish()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                while (!namedPipe.taskFinished)
                {

                }
            });
            Debug.WriteLine("Before await task");
            await task;
            Debug.WriteLine("After await task");
        }

        private async Task connectionStatus()
        {
            while (true)
            {
                if (namedPipe.clientConected)
                {
                    label3.Text = "Client conected";
                }
                else
                {
                    label3.Text = "Client disconected";
                }
                label3.Refresh();
            await Task.Delay(250);
            }
        }

        private async Task<string> option2()
        {
            string stest;
            Task<string> test = null;
            Task task = Task.Factory.StartNew(() =>
            {
                test = namedPipe.Write("Hello", 2000);
            });
            Debug.WriteLine("Before await task");
            await task;
            Debug.WriteLine("After await task");
            stest = test.Result;
            Debug.WriteLine(stest);
            return stest;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //string predictResult = "[('Saurav', 0.9100961538461538), ('there', 0.08096153846153847), ('john', 0.03005612884688044), ('rick', 0.026627029456222792), ('mary', 0.02338158208138455), ('tom', 0.020086543205082685), ('steven', 0.020012411064048), ('david', 0.01998598655213678), ('liu', 0.019972107279180352), ('zhang', 0.0199655597405337), ('wang', 0.016678330326934096), ('terry', 0.01663774711602556), ('mike', 0.013344168466130901), ('li', 0.013337512762565198), ('james', 0.013323705599554802), ('bill', 0.013319528675384043), ('laura', 0.01331025898627458), ('are', 0.001346153846153846), ('you', 0.00125), ('i', 0.0008653846153846154), ('how', 0.000673076923076923), ('hi', 0.000576923076923077), ('is', 0.0004807692307692308), ('can', 0.0004807692307692308), ('today', 0.00038461538461538467), ('sir', 0.00038461538461538467), ('hope', 0.00038461538461538467), ('feeling', 0.00038461538461538467), ('doing', 0.00038461538461538467)]/[('s', 0.10344827586206896), ('t', 0.13793103448275862), ('j', 0.06896551724137931), ('r', 0.034482758620689655), ('m', 0.06896551724137931), ('d', 0.06896551724137931), ('l', 0.10344827586206896), ('z', 0.034482758620689655), ('w', 0.034482758620689655), ('b', 0.034482758620689655), ('a', 0.034482758620689655), ('y', 0.034482758620689655), ('i', 0.06896551724137931), ('h', 0.10344827586206896), ('c', 0.034482758620689655), ('f', 0.034482758620689655)]";
            //string predictResult = "[('thee', 9.013743254339891e-09), ('hello', 0.013743254339891), ('yes', 0.113743254339891)]/[(' ', 1.0)]";
            int indexText = 0;
            int indexProbability = 1;
            try
            {
                string predictedWords = "[('the', 9.013743254339891e-09), ('hello', 0.013743254339891), ('yes', 0.113743254339891)]/[(' ', 1.0)]";
                //string predictedWords = "[('Saurav', 0.9100961538461538), ('there', 0.08096153846153847), ('john', 0.03005612884688044), ('rick', 0.026627029456222792), ('mary', 0.02338158208138455), ('tom', 0.020086543205082685), ('steven', 0.020012411064048), ('david', 0.01998598655213678), ('liu', 0.019972107279180352), ('zhang', 0.0199655597405337), ('wang', 0.016678330326934096), ('terry', 0.01663774711602556), ('mike', 0.013344168466130901), ('li', 0.013337512762565198), ('james', 0.013323705599554802), ('bill', 0.013319528675384043), ('laura', 0.01331025898627458), ('are', 0.001346153846153846), ('you', 0.00125), ('i', 0.0008653846153846154), ('how', 0.000673076923076923), ('hi', 0.000576923076923077), ('is', 0.0004807692307692308), ('can', 0.0004807692307692308), ('today', 0.00038461538461538467), ('sir', 0.00038461538461538467), ('hope', 0.00038461538461538467), ('feeling', 0.00038461538461538467), ('doing', 0.00038461538461538467)]/[('s', 0.10344827586206896), ('t', 0.13793103448275862), ('j', 0.06896551724137931), ('r', 0.034482758620689655), ('m', 0.06896551724137931), ('d', 0.06896551724137931), ('l', 0.10344827586206896), ('z', 0.034482758620689655), ('w', 0.034482758620689655), ('b', 0.034482758620689655), ('a', 0.034482758620689655), ('y', 0.034482758620689655), ('i', 0.06896551724137931), ('h', 0.10344827586206896), ('c', 0.034482758620689655), ('f', 0.034482758620689655)]";

                StringBuilder resultFullPrediction = new StringBuilder();
                List<string> predictWords = new List<string>();
                List<string> predictLetters = new List<string>();

                // The "/" separates between words and letters
                string[] splitWordsLetters = predictedWords.Split('/');
                predictWords = splitWordsLetters[0].Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
                predictLetters = splitWordsLetters[1].Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
                //The Amount of each word and letter are added as string to later being used to know how many of each would be used and later split into respective widget (Word or Letter)
                //resultFullPrediction.Append(predictWords.Count.ToString() + "," + predictLetters.Count.ToString() + ",");

                //Create Dictionary of each to set the number value as a Double
                IDictionary<string, Double> words = new Dictionary<string, Double>();
                foreach (string predict in predictWords)
                {
                    string[] values = predict.Split(',');
                    words.Add(values[indexText], Double.Parse(values[indexProbability]));
                }
                //The Double value is used to sort the probability into a list decreasent probability
                var WordsList = words.ToList();
                WordsList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                //Create Dictionary of each to set the probability number value as a Double
                IDictionary<string, Double> letters = new Dictionary<string, Double>();
                foreach (string predict in predictLetters)
                {
                    string[] values = predict.Split(',');
                    letters.Add(values[indexText], Double.Parse(values[indexProbability]));
                }
                //The Double value is used to sort the probability into a list decreasent probability
                var LetterList = letters.ToList();
                LetterList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

                //Adding all elements into one single string
                foreach (var element in WordsList)
                {
                    resultFullPrediction.Append(RemoveSpecialCharacters(element.Key) + ",");
                }
                resultFullPrediction.Append("&" + ",");
                foreach (var element in LetterList)
                {
                    resultFullPrediction.Append(RemoveSpecialCharacters(element.Key) + ",");
                }
                var singleString = resultFullPrediction.ToString();
                var test = singleString.Split('&');
                var predictedWordsList = test[0].Split(',');
                var predictedLettersList = test[1].Split(',');
                //var prediction = resultFullPrediction.ToString().Split(',');
            }
            catch (Exception)
            {

            }

            //button1.PerformClick();
            /*string predictedWordss = string.Empty;
            Process[] pname = Process.GetProcessesByName("cmd");
            Process[] pname2 = Process.GetProcessesByName("predict");
            if (pname.Length == 2 && pname2.Length == 2)
            {
                predictedWordss = sendMessage("{\"Target\" : \"APP\", \"Event\": \"END\"}");
            }
            Thread.Sleep(5000);
            Process[] pname3 = Process.GetProcessesByName("predict");
            if(pname3.Length == 0)
            {
                foreach(Process process in pname)
                {
                    process.Kill();
                    process.WaitForExit();
                    process.Dispose();
                }
            }*/


            Debug.WriteLine("hola");
        }

        string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            WordAndCharacterPredictionResponse answer = new WordAndCharacterPredictionResponse();
            string strTextBox = textBox1.Text;
            if (namedPipe.clientConected)
            {
                predictedWords = sendMessage(strTextBox);
                answer = JsonConvert.DeserializeObject<WordAndCharacterPredictionResponse>(predictedWords);

                Thread.Sleep(100);
            }
            List<string> predictWords = new List<string>();
            List<string> predictLetters = new List<string>();


            try
            {
                // The "/" separates between words and letters
                /* string[] splitWordsLetters = predictedWords.Split('/');
                 predictWords = splitWordsLetters[0].Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
                 predictLetters = splitWordsLetters[1].Split('(', ')').Where((item, index) => index % 2 != 0).ToList();*/

                predictWords = answer.PredictedWords.Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
                predictLetters = answer.NextCharacters.Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
                //The Amount of each word and letter are added as string to later being used to know how many of each would be used and later split into respective widget (Word or Letter)
                //resultFullPrediction.Append(predictWords.Count.ToString() + "," + predictLetters.Count.ToString() + ",");

                //Create Dictionary of each to set the number value as a Double
                IDictionary<string, Double> words = new Dictionary<string, Double>();
                foreach (string predict in predictWords)
                {
                    string[] values = predict.Split(',');
                    words.Add(values[0], Double.Parse(values[1]));
                }
                //The Double value is used to sort the probability into a list decreasent probability
                var WordsList = words.ToList();
                WordsList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                //Create Dictionary of each to set the probability number value as a Double
                IDictionary<string, Double> letters = new Dictionary<string, Double>();
                foreach (string predict in predictLetters)
                {
                    string[] values = predict.Split(',');
                    letters.Add(values[0], Double.Parse(values[1]));
                }
                //The Double value is used to sort the probability into a list decreasent probability
                var LetterList = letters.ToList();
                LetterList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                int i = 0;
                //Adding all elements into one single string
                string[] wordsPred = new string[WordsList.Count];
                foreach (var element in WordsList)
                {
                    wordsPred[i] = RemoveSpecialCharacters(element.Key);
                    i += 1;
                }
                i = 0;
                string[] letterPred = new string[LetterList.Count];
                foreach (var element in LetterList)
                {
                    letterPred[i] = RemoveSpecialCharacters(element.Key);
                    i += 1;
                }

                //List<string> words = predictedWords.Split(',').ToList<string>();
                textBox2.Text = String.Join(Environment.NewLine, WordsList);
                textBox3.Text = String.Join(Environment.NewLine, LetterList);
            }
            catch (Exception)
            {
                textBox2.Text = "";
                textBox3.Text = "";
            }
            
        }
    }

    
}
