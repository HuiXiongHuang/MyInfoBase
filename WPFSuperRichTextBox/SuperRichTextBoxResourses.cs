using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WPFSuperRichTextBox
{
    static class SuperRichTextBoxResourses
    {
        public static ObservableCollection<String> searchStrings = new ObservableCollection<string>();
        public static ObservableCollection<String> replaceStrings = new ObservableCollection<string>();
        public static Dictionary<String, String> SearchReplaceDictionary = new Dictionary<string, string>();
    }
}
