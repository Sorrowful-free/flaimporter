using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Assets.FlaImporter.FlaImporter

{
    [ExecuteInEditMode]
    public class TestSortString :MonoBehaviour
    {
        private void OnEnable()
        {
            var list = new List<string>
            {
                "asd_0",
                "asd_1",
                "asd_11",
                "asd_10",
                "asd_2",
                "asd_001",
                "asd_005",
                "asd_05",
                "asd_5",
                "asd_20"
            };
            var sortedList = list.OrderBy(e => e);
            foreach (var VARIABLE in sortedList)
            {
                Debug.Log(VARIABLE);
            }
        }
    }
}
