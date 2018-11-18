using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rabe_Celina_HW6.DAL;
using Rabe_Celina_HW6.Models;
using Rabe_Celina_HW6.Utilities;



namespace Rabe_Celina_HW6.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public IActionResult Index()
        {
            return View(_context.Products.ToList());
        }

        // GET: Product/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(c => c.SupplierDetails).ThenInclude(c => c.Supplier).FirstOrDefault(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        //HOW DO WE ADD THE SUPPLIER
        public async Task<IActionResult> Create(int [] SelectedSuppliers,[Bind("ProductID,SKU,Name,Price,Description")] Product product)
        {
            //finds next SKU
            product.SKU = GenerateNextSKU.GetNextSKU(_context);
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                //add connections to suppliers
                //first, find the PRODUCT you just added
                Product dbProduct = _context.Products.FirstOrDefault(c => c.SKU == product.SKU);

                //loop through selected departments and add them
                foreach (int i in SelectedSuppliers)
                {
                    Supplier dbSupplier = _context.Suppliers.Find(i);
                    SupplierDetail cd = new SupplierDetail();
                    cd.Product = dbProduct;
                    cd.Supplier = dbSupplier;
                    _context.SupplierDetails.Add(cd);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //if user doesn't specify an id
            if (id == null)
            {
                return NotFound();
            }

            //if user specifies an id tht doesn't exist
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.AllSuppliers = GetAllSuppliers();
  
            return View(product);
        }


        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product, int[] SelectedSuppliers)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //WHAT DOES THIS BLOCK MEAN
                    Product dbProduct = _context.Products
                            .Include(c => c.SupplierDetails)
                            .ThenInclude(c => c.Supplier)
                        .FirstOrDefault(c => c.ProductID == product.ProductID);

                    dbProduct.Name = product.Name;
                    dbProduct.Price = product.Price;
                    dbProduct.Description = product.Description;

                    _context.Update(dbProduct);
                    _context.SaveChanges();

                    //edit department/course relationships

                    //loop through selected departments and find ones that need to be removed
                    List<SupplierDetail> SuppsToRemove = new List<SupplierDetail>();
                    foreach (SupplierDetail cd in dbProduct.SupplierDetails)
                    {
                        if (SelectedSuppliers.Contains(cd.Supplier.SupplierID) == false)
                        //list of selected depts does not include this departments id
                        {
                            SuppsToRemove.Add(cd);
                        }
                    }
                    //remove departments you found in list above - this has to be 2 separate steps because you can't 
                    //iterate over a list that you are removing items from
                    foreach (SupplierDetail cd in SuppsToRemove)
                    {
                        _context.SupplierDetails.Remove(cd);
                        _context.SaveChanges();
                    }

                    //now add the departments that are new
                    foreach (int i in SelectedSuppliers)
                    {
                        if (dbProduct.SupplierDetails.Any(c => c.Supplier.SupplierID == i) == false)
                        //this supplier has not yet been added
                        {
                            //create a new course department
                            SupplierDetail cd = new SupplierDetail();

                            //connect the new course department to the department and course
                            cd.Supplier = _context.Suppliers.Find(i);
                            cd.Product = dbProduct;

                            //update the database
                            _context.SupplierDetails.Add(cd);
                            _context.SaveChanges();
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }



        private MultiSelectList GetAllSuppliers()
        {
            List<Supplier> AllSuppliers = _context.Suppliers.ToList();
            MultiSelectList SupplierList = new MultiSelectList(AllSuppliers, "SupplierID", "SupplierName");
            return SupplierList;
        }

        //overload for editing departments
        private MultiSelectList GetAllSuppliers(Product product)
        {
            //create a list of all the departments
            List<Supplier> allSupps = _context.Suppliers.ToList();

            //create a list for the department ids that this course already belongs to
            List<int> currentSupps = new List<int>();

            //loop through all the details to find the list of current departments
            foreach (SupplierDetail cd in product.SupplierDetails)
            {
                currentSupps.Add(cd.Supplier.SupplierID);
            }

            //create the multiselect with the overload for currently selected items
            MultiSelectList suppList = new MultiSelectList(allSupps, "SupplierID", "SupplierName", currentSupps);

            //return the list
            return suppList;
        }
    }
}
