namespace Kalkulator6
{
    public partial class Form1 : Form
    {
        float wynik;
        string wyrazenie, historia;
        bool zamiana;
        static bool blad=false;
        bool litera = false;
        string[] zmienne;
        static int iloscPrzecinkow = 0;

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count == 0)
            {
                DisableZmienna();
                litera = false;
            }
            if (richTextBox1.Text.Length > 0)
            {

                if (char.IsLetter(richTextBox1.Text[richTextBox1.Text.Length - 1]))
                {
                    litera = true;

                    ZmiennaWyrazenia(richTextBox1.Text[richTextBox1.Text.Length - 1]);
                    zmienne = new string[comboBox1.Items.Count];
                }
                if (litera == true)
                {
                    litera = false;
                    for (int i = 0; i < comboBox1.Items.Count; i++)
                    {
                        if (zmienne[i] == null)
                        {
                            litera = true;
                        }
                    }

                }

            }

            //USUWA Z COMBOBOXA LITERY KTÓRE ZOSTA£Y USUNIÊTE W RICHTEXTBOX1
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (comboBox1.Items[i] != null && richTextBox1.Text.Contains(comboBox1.Items[i].ToString()) == false)
                {
                    comboBox1.Items.Remove(comboBox1.Items[i]);
                    zmienne = new string[comboBox1.Items.Count];
                }
            }
            if (richTextBox1.Text == "" || richTextBox1.Text.Contains("=") == true || litera == true)
            { button1.Enabled = false; }
            else
            { button1.Enabled = true; }
        }

        void ZmiennaWyrazenia(char litera)
        {
            EnableZmienna();
            if (comboBox1.Items.Contains(litera) == false)
            { comboBox1.Items.Add(litera); }
        }


        public void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            RozpoczecieLiczenia();

        }


        //sekwencja uruchomienia liczenia
        private void RozpoczecieLiczenia()
        {
            string wyrazenie = richTextBox1.Text;
            wyrazenie = wyrazenie.Replace(" ", "");
            //sprawdza czy nie ma b³êdnych znaków w podanym wyra¿eniu ->dzia³a póki co
            for (int i = 0; i < wyrazenie.Length; i++)
            {
                if (char.IsLetter(wyrazenie[i]) == false && char.IsNumber(wyrazenie[i]) == false && !(wyrazenie[i] == '+' || wyrazenie[i] == '-' || wyrazenie[i] == '*' || wyrazenie[i] == '/' || wyrazenie[i] == '^' || wyrazenie[i] == '(' || wyrazenie[i] == ')' || wyrazenie[i] == ',' || wyrazenie[i] == '.'))
                {
                    wyrazenie = "";
                    break;
                }
                if (i > 0 && char.IsLetter(wyrazenie[i]) == true && char.IsLetter(wyrazenie[i - 1]) == true)
                {
                    wyrazenie = "";
                    break;
                }
            }
            //sprawdza czy nie ma b³êdnej sk³adni w podanym wyra¿eniu ->chyba dzia³a(testowaæ)
            if (wyrazenie == "" ||wyrazenie.Contains(",,")|| wyrazenie.Contains("++") || wyrazenie.Contains("+/") || wyrazenie.Contains("()") || wyrazenie.Contains("+*") || wyrazenie.Contains("+^") || wyrazenie.Contains("-+") || wyrazenie.Contains("---") || wyrazenie.Contains("-*") || wyrazenie.Contains("-/") || wyrazenie.Contains("-^") || wyrazenie.Contains("*+") || wyrazenie.Contains("*/") || wyrazenie.Contains("*^") || wyrazenie.Contains("**") || wyrazenie.Contains("/+") || wyrazenie.Contains("//") || wyrazenie.Contains("/*") || wyrazenie.Contains("/^") || wyrazenie.Contains("^+") || wyrazenie.Contains("^/") || wyrazenie.Contains("^*") || wyrazenie.Contains("^^"))
            {
                MessageBox.Show("B³êdnie podane wyra¿enie.\nSpróbuj ponownie.", "B³¹d!");
                wyrazenie = "";
                return;
            }
            wyrazenie = SprawdzanieZmiennych(richTextBox1.Text);
            Stack<string> odwrotnosc = ONP(wyrazenie);
            if (blad==true)
            {
                blad = false;
                MessageBox.Show("B³êdnie podane wyra¿enie.\nSpróbuj ponownie.", "B³¹d!");
                return;
            }
            wynik = Liczenie(odwrotnosc);
            historia = richTextBox2.Text;
            richTextBox2.Text = richTextBox1.Text.Replace(" ", "") + " = " + wynik;
            if (comboBox1.Items.Count > 0)
            {
                richTextBox2.Text = richTextBox2.Text + " dla:";
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    richTextBox2.Text = richTextBox2.Text + "\n" + comboBox1.Items[i].ToString() + " = " + zmienne[i];
                }
            }
            richTextBox2.Text = richTextBox2.Text + "\n" + historia;

        }

        //zamienia zmienne na podane liczby
        public string SprawdzanieZmiennych(string wyrazeniestring)
        {
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                wyrazeniestring = wyrazeniestring.Replace(comboBox1.Items[i].ToString(), zmienne[i].ToString());
            }
            return wyrazeniestring;
        }


        //dobra konwersja
        static Stack<string> ONP(string wyrazeniestring)
        {
            string bufor, wyrazeniePoKonwersji;
            wyrazeniePoKonwersji = wyrazeniestring;


            char[] wyrazenie = new char[wyrazeniePoKonwersji.Length];
            for (int i = 0; i < wyrazeniePoKonwersji.Length; i++)
            {
                wyrazenie[i] = wyrazeniePoKonwersji[i];

            }
            Stack<string> stos = new Stack<string>();
            Stack<char> znaki = new Stack<char>();
            Stack<string> odwrotnosc = new Stack<string>();


            //algorytm konwertuj¹cy wyra¿enie arytmetyczne do postaci Odwrotnej Adnotacji Polskiej
            for (int i = 0; i < wyrazenie.Length; i++)
            {
                //ignoruje spacje
                if (wyrazenie[i] == ' ')
                {

                }
                //sprawdza czy znakiem jest otwarcie nawiasu i od razu dodaje go do stosu znaków
                if (wyrazenie[i] == '(')
                {
                    i++;
                    stos.Push(Nawias(ref i, wyrazenie));
                }
                //tworzy liczby ujemne
                if (i == 0 && wyrazenie[i] == '-')
                {
                    stos.Push(wyrazenie[i].ToString());
                    continue;
                }
                //sprawdza czy poprzedni znak w wyra¿eniu jest liczb¹ albo kropk¹, jeœli tak to ³¹czy poprzedni znak z obecnym tworz¹c liczby wielocyfrowe
                if ((i > 0 && Liczba(ref wyrazenie[i - 1]) == true && Liczba(ref wyrazenie[i]) == true) || (i == 1 && wyrazenie[i - 1] == '-' && Liczba(ref wyrazenie[i]) == true))
                {
                    bufor = "";
                    bufor += wyrazenie[i - 1];
                    bufor += wyrazenie[i];
                    for (int j = 0; j < bufor.Length; j++)
                    {
                        iloscPrzecinkow = 0;
                        if (bufor[j] == ',')
                        {
                            iloscPrzecinkow++;
                        }
                    }
                    if (iloscPrzecinkow > 1)
                    {
                        blad = true;
                    }
                    stos.Push(bufor);
                    continue;
                }
                if (blad==true)
                { break; }
                if (i > 2 && wyrazenie[i - 1] == '-' && (wyrazenie[i - 2] == '+' || wyrazenie[i - 2] == '-' || wyrazenie[i - 2] == '*' || wyrazenie[i - 2] == '/' || wyrazenie[i - 2] == '^'))
                {
                    bufor = "";
                    bufor += wyrazenie[i - 1];
                    bufor += wyrazenie[i];
                    for(int j=0;j<bufor.Length;j++)
                    {
                        iloscPrzecinkow = 0;
                        if (bufor[j]==',')
                        {
                            iloscPrzecinkow++;
                        }
                    }
                    if (iloscPrzecinkow>1)
                    {
                        blad = true;
                    }
                    stos.Push(bufor);
                    continue;
                }
                if (blad == true)
                { break; }
                //sprawdza czy liczba jest cyfr¹/liter¹ i dodaje na stos
                else if (Liczba(ref wyrazenie[i]))
                {
                    //dodaje liczby na stos
                    stos.Push(wyrazenie[i].ToString());

                }
                //jeœli nie jest cyfr¹/liter¹ i nie ma otwarcia nawiasu to wrzuca na stos jeœli jest to znak * albo /
                else if (znaki.Count > 0)
                {
                    if (i > 1 && Liczba(ref wyrazenie[i - 1]) == false && wyrazenie[i] == '-')
                    {
                        continue;
                    }
                    if ((char)znaki.Peek() == '*' || (char)znaki.Peek() == '/' || (char)znaki.Peek() == '^')
                    {
                        stos.Push(znaki.Pop().ToString());
                    }

                }

                //powinno wrzucaæ znak + albo - do stosu jeœli nastêpnym znakiem jest + lub -
                if (znaki.Count > 0)
                {
                    if (((wyrazenie[i] == '+' || wyrazenie[i] == '-') && (znaki.Peek() == '+' || znaki.Peek() == '-')) && Liczba(ref wyrazenie[i - 1]) == true && Liczba(ref wyrazenie[i]) == false)
                    {
                        stos.Push(znaki.Pop().ToString());
                    }
                }

                //dodaje znaki + i - do stosu znaków
                if (wyrazenie[i] == '+' || wyrazenie[i] == '-')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                //dodaje znak * do stosu znaków
                if (wyrazenie[i] == '*')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                //dodaje znak / do stosu znaków
                if (wyrazenie[i] == '/')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                if (wyrazenie[i] == '^' && (stos.Peek() == "*" || stos.Peek() == "/"))
                {
                    if (stos.Peek() == "*")
                    {
                        znaki.Push('*');
                    }
                    else if (stos.Peek() == "/")
                    {
                        znaki.Push('/');
                    }
                    stos.Pop();
                    znaki.Push(wyrazenie[i]);
                }
                else if (wyrazenie[i] == '^')
                {
                    znaki.Push(wyrazenie[i]);
                }

            }
            //po zakoñczeniu pêtli for wrzuca wszystkie znaki ze stosu znaków na stos wyra¿enia
            while (znaki.Count > 0)
            {
                if ((char)znaki.Peek() == '(')
                {
                    znaki.Pop();
                }
                else
                {
                    stos.Push(znaki.Pop().ToString());
                }
            }
            //przerzuca wszystko na drugi stos, tworz¹c zapis Odwrotnej Notacji Polskiej
            while (stos.Count > 0)
            {
                odwrotnosc.Push((string)stos.Pop());
            }
            return odwrotnosc;
        }
        //dobrze liczy
        static float Liczenie(Stack<string> odwrotnosc)
        {
            float liczba1, liczba2;
            Stack<float> liczenie = new Stack<float>();
            while (odwrotnosc.Count > 0)
            {
                if (float.TryParse(odwrotnosc.Peek(), out float x))
                {
                    liczenie.Push(x);
                    odwrotnosc.Pop();
                }
                else
                {
                    if (odwrotnosc.Peek() == "+")
                    {
                        liczba2 = (float)Convert.ToDouble(liczenie.Pop());
                        liczba1 = (float)Convert.ToDouble(liczenie.Pop());
                        liczenie.Push(liczba1 + liczba2);
                        odwrotnosc.Pop();
                    }
                    else if (odwrotnosc.Peek() == "-")
                    {
                        liczba2 = (float)liczenie.Pop();
                        liczba1 = (float)liczenie.Pop();
                        liczenie.Push(liczba1 - liczba2);
                        odwrotnosc.Pop();
                    }
                    else if (odwrotnosc.Peek() == "*")
                    {
                        liczba2 = (float)liczenie.Pop();
                        liczba1 = (float)liczenie.Pop();
                        liczenie.Push(liczba1 * liczba2);
                        odwrotnosc.Pop();
                    }
                    else if (odwrotnosc.Peek() == "/")
                    {
                        liczba2 = (float)liczenie.Pop();
                        liczba1 = (float)liczenie.Pop();
                        liczenie.Push(liczba1 / liczba2);
                        odwrotnosc.Pop();
                    }
                    else if (odwrotnosc.Peek() == "^")
                    {

                        bool correctData;
                        int wykladnik;
                        correctData = int.TryParse(liczenie.Pop().ToString(), out wykladnik);
                        if (correctData == false) { Console.WriteLine("Wyk³adnik potêgi musi byæ liczb¹ ca³kowit¹!"); break; }
                        liczba1 = (float)liczenie.Pop();
                        double potegowanie = Math.Pow(liczba1, wykladnik);
                        liczenie.Push((float)potegowanie);
                        odwrotnosc.Pop();
                    }
                }
            }
            float wynik = liczenie.Pop();
            return wynik;
        }

        //sprawdza czy znak to liczba
        static bool Liczba(ref char x)
        {
            if (x == '1' || x == '2' || x == '3' || x == '4' || x == '5' || x == '6' || x == '7' || x == '8' || x == '9' || x == '0' || x == '.' || x == ',')
            {
                if (x == '.')
                {
                    x = ',';
                }
                return true;
            }
            else { return false; }
        }

        //nawias dzia³a
        static string Nawias(ref int k, char[] wyrazeniePozaNawiasem)
        {
            Stack<string> stos = new Stack<string>();
            Stack<char> znaki = new Stack<char>();
            Stack<string> odwrotnosc = new Stack<string>();
            int kopiowanie = 0;
            int iloscZnakow = 0;
            string bufor;
            int iloscNawiasow = 1;
            //sprawdza czy znakiem jest otwarcie nawiasu i od razu dodaje go do stosu znaków
            if (wyrazeniePozaNawiasem[k] == '(')
            {
                k++;
                stos.Push(Nawias(ref k, wyrazeniePozaNawiasem));//tu coœ nie gra
            }
            for (int j = k; j < wyrazeniePozaNawiasem.Length && iloscNawiasow != 0; j++)
            {
                if (wyrazeniePozaNawiasem[j] == '(') { iloscNawiasow++; }
                if (wyrazeniePozaNawiasem[j] == ')') { iloscNawiasow--; }
                iloscZnakow++;
            }
            char[] wyrazenie = new char[iloscZnakow];
            for (; kopiowanie < iloscZnakow; k++)
            {
                wyrazenie[kopiowanie] = wyrazeniePozaNawiasem[k];
                kopiowanie++;

            }

            for (int i = 0; i < wyrazenie.Length; i++)
            {
                //ignoruje spacje
                if (wyrazenie[i] == ' ')
                {

                }
                //sprawdza czy znakiem jest otwarcie nawiasu i od razu dodaje go do stosu znaków
                if (wyrazenie[i] == '(')
                {
                    i++;
                    stos.Push(Nawias(ref i, wyrazenie));
                }
                if (wyrazenie[i] == ')')
                { break; }
                //tworzy liczby ujemne
                if (i == 0 && wyrazenie[i] == '-')
                {
                    stos.Push(wyrazenie[i].ToString());
                    continue;
                }
                //sprawdza czy poprzedni znak w wyra¿eniu jest liczb¹ albo kropk¹, jeœli tak to ³¹czy poprzedni znak z obecnym tworz¹c liczby wielocyfrowe
                if ((i > 0 && Liczba(ref wyrazenie[i - 1]) == true && Liczba(ref wyrazenie[i]) == true) || (i == 1 && wyrazenie[i - 1] == '-' && Liczba(ref wyrazenie[i]) == true))
                {
                    bufor = "";
                    bufor += wyrazenie[i - 1];
                    bufor += wyrazenie[i];
                    for (int j = 0; j < bufor.Length; j++)
                    {
                        iloscPrzecinkow = 0;
                        if (bufor[j] == ',')
                        {
                            iloscPrzecinkow++;
                        }
                    }
                    if (iloscPrzecinkow > 1)
                    {
                        blad = true;
                    }
                    stos.Push(bufor);
                    continue;
                }
                if (blad == true)
                { break; }
                if (i > 2 && wyrazenie[i - 1] == '-' && (wyrazenie[i - 2] == '+' || wyrazenie[i - 2] == '-' || wyrazenie[i - 2] == '*' || wyrazenie[i - 2] == '/' || wyrazenie[i - 2] == '^'))
                {
                    bufor = "";
                    bufor += wyrazenie[i - 1];
                    bufor += wyrazenie[i];
                    for (int j = 0; j < bufor.Length; j++)
                    {
                        iloscPrzecinkow = 0;
                        if (bufor[j] == ',')
                        {
                            iloscPrzecinkow++;
                        }
                    }
                    if (iloscPrzecinkow > 1)
                    {
                        blad = true;
                    }
                    stos.Push(bufor);
                    continue;
                }
                if (blad == true)
                { break; }
                //sprawdza czy liczba jest cyfr¹/liter¹ i dodaje na stos
                else if (Liczba(ref wyrazenie[i]))
                {
                    //dodaje liczby na stos
                    stos.Push(wyrazenie[i].ToString());

                }
                //jeœli nie jest cyfr¹/liter¹ i nie ma otwarcia nawiasu to wrzuca na stos jeœli jest to znak * albo /
                else if (znaki.Count > 0)
                {
                    if (i > 1 && Liczba(ref wyrazenie[i - 1]) == false && wyrazenie[i] == '-')
                    {
                        continue;
                    }
                    if ((char)znaki.Peek() == '*' || (char)znaki.Peek() == '/' || (char)znaki.Peek() == '^')
                    {
                        stos.Push(znaki.Pop().ToString());
                    }

                }

                //powinno wrzucaæ znak + albo - do stosu jeœli nastêpnym znakiem jest + lub -
                if (znaki.Count > 0)
                {
                    if (((wyrazenie[i] == '+' || wyrazenie[i] == '-') && (znaki.Peek() == '+' || znaki.Peek() == '-')) && Liczba(ref wyrazenie[i - 1]) == true && Liczba(ref wyrazenie[i]) == false)
                    {
                        stos.Push(znaki.Pop().ToString());
                    }
                }

                //dodaje znaki + i - do stosu znaków
                if (wyrazenie[i] == '+' || wyrazenie[i] == '-')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                //dodaje znak * do stosu znaków
                if (wyrazenie[i] == '*')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                //dodaje znak / do stosu znaków
                if (wyrazenie[i] == '/')
                {
                    znaki.Push((char)wyrazenie[i]);
                }
                if (wyrazenie[i] == '^' && (stos.Peek() == "*" || stos.Peek() == "/"))
                {
                    if (stos.Peek() == "*")
                    {
                        znaki.Push('*');
                    }
                    else if (stos.Peek() == "/")
                    {
                        znaki.Push('/');
                    }
                    stos.Pop();
                    znaki.Push(wyrazenie[i]);
                }
                else if (wyrazenie[i] == '^')
                {
                    znaki.Push(wyrazenie[i]);
                }

            }

            //po zakoñczeniu pêtli for wrzuca wszystkie znaki ze stosu znaków na stos wyra¿enia
            while (znaki.Count > 0)
            {
                if ((char)znaki.Peek() == '(')
                {
                    znaki.Pop();
                }
                else
                {
                    stos.Push(znaki.Pop().ToString());
                }
            }
            while (stos.Count > 0)
            {
                odwrotnosc.Push((string)stos.Pop());
            }
            string wynik = Liczenie(odwrotnosc).ToString();
            return wynik;
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (button1.Enabled)
                {
                    RozpoczecieLiczenia();
                }
            }
        }


        //zapisywanie zmiennych, dzia³a ->zaimplementowaæ
        private void button2_Click(object sender, EventArgs e)
        {
            PrzypisanieZmiennych();

        }
        public void PrzypisanieZmiennych()
        {
            bool sprawdzanieDanych = true;
            float x;
            richTextBox4.Text = richTextBox4.Text.Replace(" ", "");
            richTextBox4.Text = richTextBox4.Text.Replace(".", ",");
            sprawdzanieDanych = float.TryParse(richTextBox4.Text, out x);
            if (sprawdzanieDanych == false)
            {
                MessageBox.Show("Podana b³êdna zmienna!", "B³¹d!");
                return;
            }
            zmienne[comboBox1.SelectedIndex] = x.ToString();
            richTextBox3.Text = "";
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                richTextBox3.Text = richTextBox3.Text + comboBox1.Items[i].ToString() + " = " + zmienne[i] + "\n";
            }
            if (litera == true)
            {
                litera = false;
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (zmienne[i] == null)
                    {
                        litera = true;
                    }
                }

            }
            if (richTextBox1.Text == "" || richTextBox1.Text.Contains("=") == true || litera == true)
            { button1.Enabled = false; }
            else
            { button1.Enabled = true; }
        }

        //pokazuje/ukrywa UI do zmiany zmiennych
        public void EnableZmienna()
        {
            label4.Visible = true;
            comboBox1.Visible = true;
            button2.Visible = true;
            richTextBox3.Visible = true;
            richTextBox4.Visible = true;
        }
        public void DisableZmienna()
        {
            label4.Visible = false;
            comboBox1.Visible = false;
            button2.Visible = false;
            richTextBox3.Visible = false;
            richTextBox4.Visible = false;
        }

        private void richTextBox4_KeyDown(object sender, KeyEventArgs e)
        {
            string ch = e.KeyCode.ToString();
            if (char.IsLetter(ch[0]))
            {
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                PrzypisanieZmiennych();
                if (button1.Enabled)
                {
                    RozpoczecieLiczenia();
                }
            }

        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (!(e.KeyChar == '.' || e.KeyChar == ','))
                    e.Handled = true;
            }

        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //uniemo¿liwia wpisanie nieu¿ywanych znaków
            if (!(Char.IsLetter(e.KeyChar))&& (!(Char.IsDigit(e.KeyChar))&& !(e.KeyChar == '+' || e.KeyChar == '-' || e.KeyChar == '*' || e.KeyChar == '/' || e.KeyChar == '^' || e.KeyChar == '(' || e.KeyChar == ')' || e.KeyChar == ',' || e.KeyChar == '.'))
)
            {
                e.Handled = true;
            }
            if (e.KeyChar=='.')
            {
                e.KeyChar = ',';
            }
        }

        //blokuje mo¿liwoœæ rêcznego wpisania wartoœci w comboBoxie
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                e.Handled = true;
            }
        }

    }
}