using System;
using System.Linq;

namespace ThinkAway.Plus.Update
{
    public class NewVersionEventArgs : EventArgs
    {
        public Products Products;

        public Product NewVersion { get; set; }

        public NewVersionEventArgs(Products products)
        {
            products.Sort((obj, obj1) => (new Version(obj.ProductVersion) < new Version(obj1.ProductVersion)) ? +1 : -1);

            this.Products = products;
            this.NewVersion = products.FirstOrDefault();
        }
    }
}