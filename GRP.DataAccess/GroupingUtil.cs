//using GRP.Models.GeneralSettings.DeptsAndCenters;
using GrpSample.Models.System;
using System.Collections.Generic;
using System.Linq;
using System;
//using GrpSample.Models.HAJ.HajjDeptsAndCenters;

namespace GrpSample.DataAccess
{
    public static class GroupingUtil
    {
        public static IList<TreeNode> BuildFormTree(IEnumerable<SysForm> AllData)
        {
            if (AllData != null && AllData.Any())
            {
                var groups = AllData.GroupBy(i => i.sys_Parent);

                var roots = groups.FirstOrDefault(g => g.Key == null).Select(sf => new TreeNode()
                {
                    id = sf.sys_Id,
                    text = sf.sys_Ar_Name,
                    state = "closed"
                }).ToList();

                if (roots.Count > 0)
                {
                    var dict = groups.Where(g => g.Key != null).ToDictionary(g => g.Key, g => g.ToList());
                    for (int i = 0; i < roots.Count; i++)
                        AddChildren(roots[i], dict);
                }

                return roots;
            }
            else
                return new List<TreeNode>();
        }

        private static void AddChildren(TreeNode FormItem, IDictionary<string, List<SysForm>> DataSource)
        {
            if (DataSource.ContainsKey(FormItem.id.ToString()))
            {
                FormItem.state = "closed";
                FormItem.children = DataSource[FormItem.id.ToString()].Select(sf => new TreeNode()
                {
                    id = sf.sys_Id,
                    text = sf.sys_Ar_Name,
                    attributes = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", sf.env_Flag, sf.sys_Id, sf.sys_En_Name,
                                        string.IsNullOrWhiteSpace(sf.sys_path) ? string.Empty : sf.sys_path,
                                        sf.sys_Type, sf.ext_sys_path)
                }).ToList();

                for (int i = 0; i < FormItem.children.Count; i++)
                    AddChildren(FormItem.children[i], DataSource);
            }
            else
            {
                FormItem.children = new List<TreeNode>();
            }
        }

        //public static IList<TreeNode> BuildDeptsTree(IEnumerable<DeptMainInfo> AllData)
        //{
        //    var groups = AllData.GroupBy(i => i.SP);

        //    var roots = groups.FirstOrDefault(g => g.Key == null).Select(sf => new TreeNode()
        //    {
        //        id = sf.SL,
        //        text = sf.DN,
        //        iconCls = GetDeptIconName(true, sf.DT, sf.CF),
        //        state = "closed"
        //    }).ToList();

        //    if (roots.Count > 0)
        //    {
        //        var dict = groups.Where(g => g.Key != null).ToDictionary(g => g.Key, g => g.ToList());
        //        for (int i = 0; i < roots.Count; i++)
        //            AddChildren(roots[i], dict);
        //    }

        //    return roots;
        //}

        //Added for Haj Depts Tree
        //public static IList<TreeNode> BuildHajDeptsTree(IEnumerable<GRP.Models.HAJ.HajjDeptsAndCenters.DeptMainInfo_Hajj> AllData)
        //{
        //    var groups = AllData.GroupBy(i => i.SP);

        //    var roots = groups.FirstOrDefault(g => g.Key != null).Select(sf => new TreeNode()
        //    {
        //        id = sf.SL,
        //        text = sf.DN,
        //        iconCls = GetDeptIconName(true, sf.DT, sf.CF),
        //        state = "closed"
        //    }).ToList();

        //    if (roots.Count > 0)
        //    {
        //        var dict = groups.Where(g => g.Key != null).ToDictionary(g => g.Key, g => g.ToList());
        //        for (int i = 0; i < roots.Count; i++)
        //            AddChildren(roots[i], dict);
        //    }

        //    return roots;
        //}

        //Added for GRP.Models.HAJ.HajjDeptsAndCenters.DeptMainInfo_Hajj
        //private static void AddChildren(TreeNode FormItem, IDictionary<string, List<DeptMainInfo_Hajj>> DataSource)
        //{
        //    if (DataSource.ContainsKey(FormItem.id.ToString()))
        //    {
        //        FormItem.state = "closed";
        //        FormItem.children = DataSource[FormItem.id.ToString()].Select(sf => new TreeNode()
        //        {
        //            id = sf.SL,
        //            iconCls = GetDeptIconName(false, sf.DT, sf.CF),
        //            text = sf.DN
        //        }).ToList();

        //        for (int i = 0; i < FormItem.children.Count; i++)
        //            AddChildren(FormItem.children[i], DataSource);
        //    }
        //    else
        //    {
        //        FormItem.children = new List<TreeNode>();
        //    }
        //}

        //private static void AddChildren(TreeNode FormItem, IDictionary<string, List<DeptMainInfo>> DataSource)
        //{
        //    if (DataSource.ContainsKey(FormItem.id.ToString()))
        //    {
        //        FormItem.state = "closed";
        //        FormItem.children = DataSource[FormItem.id.ToString()].Select(sf => new TreeNode()
        //        {
        //            id = sf.SL,
        //            iconCls = GetDeptIconName(false, sf.DT, sf.CF),
        //            text = sf.DN
        //        }).ToList();

        //        for (int i = 0; i < FormItem.children.Count; i++)
        //            AddChildren(FormItem.children[i], DataSource);
        //    }
        //    else
        //    {
        //        FormItem.children = new List<TreeNode>();
        //    }
        //}

        public static List<TreeNode> GenerateUITree<T>(IEnumerable<T> DataList, Func<T, int> IdSelector, Func<T, int> ParentIdSelector, Func<T, string> TextFunc, int ParentId = 0, Func<T, string> IconFunc = null, Func<T, string> StateFunc = null, Func<T, string> AttributesFunc = null)
        {
            var childNodes = DataList.Where(x => ParentIdSelector(x) == ParentId);
            return childNodes.Select(x => new TreeNode
            {
                id = IdSelector(x),
                text = TextFunc(x),
                iconCls = IconFunc == null ? string.Empty : IconFunc(x),
                state = StateFunc == null ? "closed" : StateFunc(x),
                attributes = AttributesFunc == null ? string.Empty : AttributesFunc(x),
                children = GenerateUITree<T>(DataList, IdSelector, ParentIdSelector, TextFunc, IdSelector(x), IconFunc, StateFunc, AttributesFunc)
            }).ToList();
        }

        /*private List<thing> GetItems(Func<thing, bool> filter = null)
        {
            return rawList.Where(filter ?? (s => true)).ToList();
        }*/

        public static string GetDeptIconName(bool IsParent, string DeptType, string CenterFlag)
        {
            //.icon-dept-branch .icon-large-dept-branch
            //.icon-dept-civilLead .icon-large-dept-civilLead 
            //.icon-dept-mainDept .icon-large-dept-mainDept 
            //.icon-dept-subDept .icon-large-dept-subDept 
            //.icon-dept-medCenter .icon-large-dept-medCenter 
            //.icon-dept-oprRoom .icon-large-dept-oprRoom 

            string retVal = IsParent ? "tree-folder" : "tree-file";

            if (CenterFlag == "Y" && DeptType != "R")
                retVal = "icon-dept-medCenter";
            else if (DeptType == "B")
                retVal = "icon-dept-branch";
            else if (DeptType == "M")
                retVal = "icon-dept-mainDept";
            else if (DeptType == "T")
                retVal = "icon-dept-subDept";
            else if (DeptType == "R")
                retVal = "icon-dept-oprRoom";
            else if (DeptType == "G")
                retVal = "icon-dept-civilLead";

            return retVal;
        }
    }
}