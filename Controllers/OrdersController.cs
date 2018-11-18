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
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Order
        //when you go to /order, "homepage" of orders
        //IActionResult returns a view
        public IActionResult Index()
        {
            //instead of var, use class type Order order??
            //create order object WHY DO WE INCLUDE ORDERDETAILS AND PROJECT
            var order = _context.Orders.Include(x => x.OrderDetails)
                                .ThenInclude(x => x.Product).ToList();
            return View(order);
        }

        // GET: Order/Details/5
        //Async Task doesn't run the code in order. don't have to wait for one line to finish
        //takes a an int as a parameter
        //asyncronous.
        public async Task<IActionResult> Details(int? id)
        {

            //if no orderID was chosen
            if (id == null)
            {
                return NotFound();
            }

            //create an order object
            //include Product (navigational property)
            //and find the first instance of orderID that matches the int that was passed
            var order = await _context.Orders.Include(r => r.OrderDetails).ThenInclude(r => r.Product).FirstOrDefaultAsync(m => m.OrderID == id);

            //if orderID doesn't correlate with an order
            if (order == null)
            {
                return NotFound();
            }

            //send order object to the Orders/Details view
            return View(order);
            }
        
       
        // GET: Order/Create
        public IActionResult Create()
        {
            //send all products so view can populate the drop down list
            ViewBag.AllProducts = GetAllProducts();
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //takes variables from view input and order object as parameters
        public async Task<IActionResult> Create([Bind("OrderID,OrderNumber,OrderDate,Notes")] Order order)
        {
            //get next order
            order.OrderNumber = GenerateNextOrderNumber.GetNextOrderNumber(_context);
            //get order date
            order.OrderDate = System.DateTime.Today;
            //if the inputs fit the property requirements 
            if (ModelState.IsValid)
            {
                //add order object to model
                _context.Add(order);
                //save changes to the model
                await _context.SaveChangesAsync();
                //go to add to order view, passing the orderID int
                return RedirectToAction("AddToOrder", new { id = order.OrderID });
            }
            return View(order);
        }

        //GET
        public IActionResult AddToOrder(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "You must specify an order to add!" });
            }
            Order reg = _context.Orders.Find(id);
            if (reg == null)
            {
                return View("Error", new string[] { "Order not found!" });
            }

            OrderDetail rd = new OrderDetail() { Order = reg };

            ViewBag.AllProducts = GetAllProducts();
            return View("AddToOrder", rd);
        }

        [HttpPost]
        public IActionResult AddToOrder(OrderDetail rd, int SelectedProduct)
        {
            //find the course associated with the selected course id
            Product product = _context.Products.Find(SelectedProduct);
            //set the registration detail's course equal to the course we just found
            rd.Product = product;

            //find the registration based on the id
            Order reg = _context.Orders.Find(rd.Order.OrderID);

            //set the registration detail's registration equal to the registration we just found
            rd.Order = reg;

            //set the course fee for this detail equal to the current course fee
            rd.ProductPrice = rd.Product.Price;

            //add total fees
            rd.ExtendedPrice = rd.ProductPrice * rd.QuantityOrdered;



            if (ModelState.IsValid)
            {
                _context.OrderDetails.Add(rd);
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = rd.Order.OrderID });
            }
            ViewBag.AllProducts = GetAllProducts();
            return View(rd);
        }


        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Orders
                                        .Include(r => r.OrderDetails)
                                            .ThenInclude(r => r.Product)
                                        .FirstOrDefault(r => r.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Order order)
        {
            //Find the related registration in the database
            Order DbReg = _context.Orders.Find(order.OrderID);

            //Update the notes
            DbReg.Notes = order.Notes;

            //Update the database
            _context.Orders.Update(DbReg);

            //Save changes
            _context.SaveChanges();

            //Go back to index
            return RedirectToAction(nameof(Index));
        }





        public IActionResult RemoveFromOrder(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "You need to specify an order id" });
            }

            Order reg = _context.Orders.Include(r => r.OrderDetails).ThenInclude(r => r.Product).FirstOrDefault(r => r.OrderID == id);

            if (reg == null || reg.OrderDetails.Count == 0)//registration is not found
            {
                return RedirectToAction("Details", new { id = id });
            }

            //pass the list of order details to the view
            return View(reg.OrderDetails);
        }







        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }

        private SelectList GetAllProducts()
        {
            List<Product> products = _context.Products.ToList();
            SelectList allProducts = new SelectList(products, "ProductID", "Name");
            return allProducts;
        }

    }
}
