﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2
{
    public class Shop
    {
	    public string shopTag = "";
	    public string shopName = "";
	    public List<ItemRefs> shopItemRefs = new List<ItemRefs>();
	    public List<ItemRefs> initialShopItemRefs = new List<ItemRefs>();
    
        public Shop()
	    {
		
	    }
    
        public bool containsItemWithResRef(String resref)
	    {
		    foreach (ItemRefs i in this.shopItemRefs)
		    {
			    if (i.resref.Equals(resref)) { return true; }
		    }
		    return false;
	    }
    
        public bool containsInitialItemWithResRef(String resref)
	    {
		    foreach (ItemRefs i in this.initialShopItemRefs)
		    {
			    if (i.resref.Equals(resref)) { return true; }
		    }
		    return false;
	    }
    
        public Shop DeepCopy()
        {
            Shop copy = new Shop();
		    copy.shopTag = this.shopTag;
		    copy.shopName = this.shopName;
		    copy.shopItemRefs = new List<ItemRefs>();
		    foreach (ItemRefs s in this.shopItemRefs)
            {
                copy.shopItemRefs.Add(s);
            }
		    return copy;
        }
    }
}
