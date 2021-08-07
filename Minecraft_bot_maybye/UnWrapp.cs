using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_bot_maybye
{
    class UnWrapp
    {
        public static string[] ToIndividual(string input)
        {
            //out of range
            int startOfFinding = 0;

            int loopcount = 0;
            foreach(char ch in input)
            {
                if(ch == 'i')
                {
                    loopcount++;
                }
            }
            string[] result= new string[loopcount];
            for (int i = 0; i<= loopcount-1;i++)
            {
                int towrap = input.IndexOf("/j", startOfFinding);
                string sub = input.Substring(startOfFinding, towrap + 2);
                input = input.Substring(towrap + 2);
                result[i] = sub;
            }
            return result;
        }
        public static double ToNum(string input, char ch)
        {
            if (ch.Equals('a'))
            {
                int indexOfCh = input.IndexOf(ch);
                string sub = input.Substring(0, indexOfCh - 1);
                return Convert.ToDouble(sub);
            }
            else
            {
                char prevchar = ch.Equals('b') ? 'a' : (ch.Equals('c') ? 'b' : (ch.Equals('d') ? 'c' : (ch.Equals('e') ? 'd' : (ch.Equals('f') ? 'e' : (ch.Equals('g') ? 'f' : (ch.Equals('h') ? 'g' : (ch.Equals('i') ? 'h' : (ch.Equals('j') ? 'i' : 'a'))))))));
                int indexOfCh = input.IndexOf(ch);
                int indexOfPrevCh = input.IndexOf(prevchar);
                string sub = input.Substring(indexOfPrevCh+1, indexOfCh - indexOfPrevCh-2);
                return Convert.ToDouble(sub);
            }
        }
    }
}
