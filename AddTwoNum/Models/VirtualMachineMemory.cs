using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;


namespace AddTwoNum.Models
{  
    public class VirtualMachineMemory
    {
        
        public VirtualMachineMemory()
        {
        }

       
        Hashtable _memory = new Hashtable();
        List<string> keys;
        List<int> keys_int;


        public Hashtable Memory
        {

            get {return _memory; }
            
        }

        
        public List<int> getKeys()
        {
            keys = _memory.Keys.Cast<string>().ToList();           
            keys_int = keys.Select(int.Parse).ToList();
            keys_int.Sort();
            return keys_int;
        }


    }
}