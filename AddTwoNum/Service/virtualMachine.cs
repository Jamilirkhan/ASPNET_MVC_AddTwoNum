using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AddTwoNum.Models;
using System.Text.RegularExpressions;
using System.Collections;


namespace AddTwoNum.Service
{
    public sealed class virtualMachine
    {
        
        #region MAIN_CLASS

        private VirtualMachineMemory vm_data;
        ArrayList statements;
        Hashtable symboltable;

        private static readonly virtualMachine instance = new virtualMachine();

        private virtualMachine()
        {
            
        }

        public static virtualMachine Instance
        {
            get
            {
                return instance;
            }
        }


        public void startUp()
        {
            if (vm_data == null)
            {
                Random rand = new Random();
                vm_data = new VirtualMachineMemory();

                for (int i = 0; i < 5; i++)
                {
                    this.setCell(i.ToString(), (rand.Next(1000)));
                }

            }

        }
        public bool ExecueScript(string script)
        {
            bool parseSuccess = false;
            statements = new ArrayList();
            symboltable = new Hashtable();
            parseSuccess = parseScript(script);
            return parseSuccess;
        }

        public bool ResetMemory()
        {          
            statements = new ArrayList();
            symboltable = new Hashtable();
            vm_data = null;
            startUp();          
            return true;
        }
        

        #endregion

        #region MEMORY_MANIPULATION


        public void incrementCell(string key)
        {
            int cell;
            cell = (this.getCell(key));
            cell++;
            this.setCell(key, cell);
        }

        public void setCell(string key, int value)
        {
            vm_data.Memory[key] = value.ToString();            
        }
        public void addCell(string key, int value)
        {
            vm_data.Memory.Add(key, value);            
        }

        public int getCell(string key)
        {
            return Convert.ToInt32(vm_data.Memory[key]);
        }


        public VirtualMachineMemory vm_Data
        {
            get { return this.vm_data; }                      
        }

        #endregion

        #region SCRIPT_PROCESSOR
       

        public bool parseScript(string script)
        {
            int statement_Line_Number=0;
            bool parseSuccess = false;
            

            foreach (var line in script_processor.SplitToLines(script))
            {   // one line at a time...
                statements.Add(line.ToString().Trim());
            }

           
            foreach (string s in statements)
            {
                statement_Line_Number++;
                parseSuccess = parseStatement(s, statement_Line_Number);
                if (parseSuccess == false)
                    break;
            }


            return parseSuccess;

        }
        public bool parseStatement(string statement,int stm_LineNumber)
        {
            bool parseStatementSuccess = false;
            char[] delimiterChars = { ',', ' ' , ':'};


            string[] lexemes;

            lexemes = statement.Split(delimiterChars);

            if (lexemes[0] == 'Z'.ToString())
            {
                parseStatementSuccess = parseZ(statement);
            }
            else if (lexemes[0] == "I")
            {
                parseStatementSuccess = parseI(statement);

            }
            else if (lexemes[0] == "J")
            {
                parseStatementSuccess = parseJ(statement, stm_LineNumber);
            }
            else if (validLabel(lexemes[0]))
            {
                parseStatementSuccess = parseLabel(statement, stm_LineNumber);
            }
            else
            {
                return false;
            }

            
            return parseStatementSuccess;

        }

        private bool validLabel(string label)
        {
            return IdentifierExtensions.IsValidIdentifier(label);
            //return true;
        }


        private bool validDigit(string number)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(number.ToString(), "^[0-9]{1,5}$"))
                return true;
            else
                return false;
        }

        private bool parseZ(string zStamt)
        {
            char[] delimiterChars = {' '};


            string[] lexemes;

            lexemes = zStamt.Split(delimiterChars);
            //checking if it is syntactically valid Z statement
            if (lexemes.Length == 2 && lexemes[0] == "Z" && validDigit(lexemes[1]))
            {
                //Here setting the given cell# to zero
                if (vm_data.Memory.Contains(lexemes[1].ToString()))
                    setCell(lexemes[1].ToString(), 0);
                else
                    addCell(lexemes[1].ToString(), 0);
            }
            else
                return false;

            return true;
        }

        private bool parseLabel(string labelStamt, int lineNumber)
        {
            bool parseStatementSuccess=false;
            char[] delimiterChars = { ':', ' ' };
            Random rand = new Random();
            string statement;//this will contain the actual statement to execute after truncating label from the beginning of the statement
            int indexOfcolon = 0;

            string[] lexemes;

            indexOfcolon=labelStamt.IndexOf(':');

            statement=labelStamt.Substring(indexOfcolon + 2);

            lexemes = labelStamt.Split(delimiterChars);
            
            if (lexemes.Length == 4 && (lexemes[2]== "Z" || lexemes[2]== "I" ) && validDigit(lexemes[3]))
            {   //if true then it means that the statement with label is syntactically correct    

                if(symboltable.ContainsKey(lexemes[0].ToString()) && Convert.ToInt32(symboltable[lexemes[0].ToString()]) == lineNumber+1)
                {   //if true then it means this statement is called due to a J (jump) statement
                    
                    if (lexemes[2] == "Z")
                    {
                        parseStatementSuccess = parseZ(statement);
                    }
                    else if (lexemes[2] == "I")
                    {
                        parseStatementSuccess = parseI(statement);
                    }
                }
                else if (symboltable.ContainsKey(lexemes[0].ToString()))
                { // if true then it means  same label is used second time which is not allowed
                    return false;
                }
                else
                {
                    symboltable[lexemes[0].ToString()] = lineNumber;
                    if(lexemes[2] == "Z")
                    {
                        parseStatementSuccess = parseZ(statement);
                    }
                    else if(lexemes[2] == "I")
                    {
                        parseStatementSuccess = parseI(statement);
                    }

                }
                    
            }
            else
                return false;
            return true;
            
        }
        private bool parseI(string iStamt)
        {
            char[] delimiterChars = { ' ' };
            Random rand = new Random();

            string[] lexemes;

            lexemes = iStamt.Split(delimiterChars);
            //if it is syntactically valid I statement
            if (lexemes.Length == 2 && lexemes[0] == "I" && validDigit(lexemes[1]))
            {
                //Here setting the given cell# to zero
                if (vm_data.Memory.Contains(lexemes[1].ToString()))
                    incrementCell(lexemes[1].ToString());                    
                else
                {
                    addCell(lexemes[1].ToString(), (rand.Next(1000)));
                    incrementCell(lexemes[1].ToString());
                }
                    
            }
            else
                return false;

            return true;
        }
        private bool parseJ(string jStamt, int stm_LineNumber_of_this_jumpStmt)
        {
            bool parseSuccess=false;
            char[] delimiterChars = { ' ', ',' };
            Random rand = new Random();

            string[] lexemes;

            lexemes = jStamt.Split(delimiterChars);
            //if it is syntactically valid I statement
            if (lexemes.Length == 4 && lexemes[0] == "J" && validDigit(lexemes[1]) && validDigit(lexemes[2]) && validLabel(lexemes[3]))
            {
                if(!symboltable.ContainsKey(lexemes[3]))
                {   //it is mandatory that the label where J statement intends to jump to, should have occured earlier in the statements
                    return false;
                }
                else
                {
                    int lhs = 0;
                    int rhs = 0;
                    lhs = this.getCell(lexemes[1].ToString());
                    rhs = this.getCell(lexemes[2].ToString());

                    int stmt_LineNumber_toStartLoop;
                    string stmt_toStartLoop;
                    string key;
                    //int statement_Line_Number;
                    
                    key = lexemes[3].ToString();

                    stmt_LineNumber_toStartLoop = (int)symboltable[key];
                    stmt_LineNumber_toStartLoop--; // we decrement by 1 because the statements in the "statements" ArrayList has indexes starting from 0

                    while (lhs!= rhs)// this while loop is implementting the check ( if (memory[n]!=memory[m])  goto label; )
                    {
                        
                        //statement_Line_Number=stmt_LineNumber_toStartLoop;

                        //this for loop is actually the loop that runs the statements between the Lable and jump statement
                        for(int j= stmt_LineNumber_toStartLoop; j < (stm_LineNumber_of_this_jumpStmt - 1); j++)// string s in statements)
                        {                            
                            parseSuccess = parseStatement(statements[j].ToString(), j);
                            if (parseSuccess == false)
                                break;                            
                        }
                        if (parseSuccess == false)
                            break;

                        lhs = this.getCell(lexemes[1].ToString());
                        rhs = this.getCell(lexemes[2].ToString());
                    }
                }

            }
            else
                return false;
            return true;

        }
      
        #endregion

    }

    #region SCRIPT_ITERATOR
    public static class script_processor
    {
        
        public static IEnumerable<string> SplitToLines(this string input)
        {
            if (input == null)
            {
                yield break;
            }

            using (System.IO.StringReader reader = new System.IO.StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line != "" && line[0] != '#')
                        yield return line;
                }
            }
        }
    }
    #endregion SCRIPT_ITERATOR

}