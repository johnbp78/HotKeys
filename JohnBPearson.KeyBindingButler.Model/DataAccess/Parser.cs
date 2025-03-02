﻿
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JohnBPearson.KeyBindingButler.Model;

namespace JohnBPearson.com.Utility
{
    internal class Parser
    {



        private class Resurrect
        {

            const int Delimiter = 25;


            internal Resurrect()
            {
            }
        
        }

        private IKeyBoundDataList _parent;
        internal Parser(KeyAndDataStringLiterals strings, IKeyBoundDataList parent)
        {

            this._valuesString = strings.Values;

            this._keysString = strings.Keys;
            this._descriptionString = strings.Descriptions;
            this._parent = parent;

        }

        private string _keysString;
        private string _valuesString;
        private string _descriptionString;

        private List<JohnBPearson.KeyBindingButler.Model.IKeyBoundData> _items;

        internal List<JohnBPearson.KeyBindingButler.Model.IKeyBoundData> Items
        {
            get
            {
                if (this._items == null)
                {
                    this.setParsed();
                }                return this._items;
            }
            private set { this._items = value; }
        }

        private List<string> _keys;

        internal List<string> Keys
        {
            get
            {
                if (this._keys == null)
                {
                    this.setParsed();
                }
                return this._keys;
            }
            private set { _keys = value; }
        }

        private void setParsed()
        {
            this._items = this.parse();

        }

        internal static IEnumerable<string> parseStringToList(string value, char delim)
        {
            var list = new List<string>();

            if (value.Contains(delim))
            {
                var arr = new List<char>();
                    arr.Add(delim);
                    
                  list.AddRange(  value.Split(arr.ToArray(), StringSplitOptions.None));
                return list;
            }


            throw new ArgumentException(string.Concat( delim, " was not found in  ", value));
        }

        private List<JohnBPearson.KeyBindingButler.Model.IKeyBoundData> parse()
        {
            string[] delims = { delim };
            var resultList = new List<IKeyBoundData>();
            //   var letters = this._keysString.Split(delims, 100, StringSplitOptions.None).Clone();
            var letters = this._keysString.Split(delimChar).Clone();
            var values = this._valuesString.Split(delimChar);
            var descriptions = this._descriptionString.Split(delimChar);
            this._keys = (letters as string[]).ToList();
            var index = 0;
            foreach (var key in this._keys)
            {
                if (index < values.Length)
                {
                    var value = values[index];
                    var des = descriptions[index];
                    var hkv = JohnBPearson.KeyBindingButler.Model.KeyBoundData.Create(this._parent,key[0], value, des);
                    resultList.Add(hkv);
                    index++;
                }
                else
                {
                    break;
                }

            }
            checkAndRepairValuesArray(values);
            checkAndRepairValuesArray(descriptions);

            return resultList;
        }

        private string[] checkAndRepairValuesArray(string[] values)
        {
            if (values.Length < 26)
            {
             var needToAdd = 26- values.Length;
                
                for (int i = 0; i < needToAdd; i++)
                {
                    values.Append("");
                }
            }
            return values;
        }

        private const string delim = "|";
        private const char delimChar = '|';
        internal KeyAndDataStringLiterals updateStrings(List<JohnBPearson.KeyBindingButler.Model.IKeyBoundData> items)
        {
            var result = 0;
            var tempKeys = new List<string>();
            var tempValues = new List<string>();
            var tempDescs = new List<string>();
            var values = new StringBuilder();
            foreach (var item in this.Items)
            {
                if (item.ObjectState != ObjectState.Deleted)
                {
                    if (!item.setIfLastItem()) { 
                    tempKeys.Add(item.Key.GetDeliminated());
                        tempValues.Add(item.Data.GetDeliminated());
                        tempDescs.Add(item.Description.GetDeliminated());
                    }else
                    {
                        tempKeys.Add(item.Key.Value);
                        tempValues.Add(item.Data.Value);
                        tempDescs.Add(item.Description.Value);
                    }
                }
                if(item.ObjectState == ObjectState.Mutated)
                {
                    result++;
                }
                else if(item.ObjectState == ObjectState.Deleted)
                {
                    tempKeys.Add(item.Key.GetDeliminated());
                    tempValues.Add(item.Data.GetDeliminated());
                    tempDescs.Add(item.Description.GetDeliminated());
                }
            }
            var strings = new KeyAndDataStringLiterals();
          
            strings.Keys = tempKeys.ToString();
            strings.Descriptions = tempDescs.ToString();
            strings.Values = this.checkAndRepairValuesArray(tempValues.ToArray()).ToString();
            return strings;
        }


    }
}


