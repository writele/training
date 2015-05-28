﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ActiveCommerce.Carts;
using ActiveCommerce.Orders;
using ActiveCommerce.Orders.Pipelines;
using ActiveCommerce.Payment;
using ActiveCommerce.Training.OrderExtension;
using ActiveCommerce.Training.Payment.CheckOut;
using Sitecore.Ecommerce.DomainModel.CheckOuts;

namespace ActiveCommerce.Training.Payment.OrderProcessing
{
    public class VerifyPayment : ActiveCommerce.Orders.Pipelines.CompleteOrderProcessing.VerifyPayment
    {
        protected override PaymentMeans CreatePaymentMeans(OrderPipelineArgs args)
        {
            var purchaseOrderPayment = GetPurchaseOrderPayment(args);
            return purchaseOrderPayment ?? base.CreatePaymentMeans(args);
        }

        protected virtual PurchaseOrderPaymentMeans GetPurchaseOrderPayment(OrderPipelineArgs args)
        {
            if (!(args.PaymentProvider is InvoicePaymentOption))
            {
                return null;
            }

            var checkout = Sitecore.Ecommerce.Context.Entity.GetInstance<ICheckOut>() as IInvoicePayment;
            if (checkout == null)
            {
                Sitecore.Diagnostics.Log.Warn(string.Format("Could not find {0} checkout data when saving purchase order data", typeof(IInvoicePayment)), this);
                return null;
            }

            if (string.IsNullOrWhiteSpace(checkout.PurchaseOrderNumber))
            {
                Sitecore.Diagnostics.Log.Warn("Empty purchase order number when saving purchase order data", this);
                return null;
            }

            return new PurchaseOrderPaymentMeans
            {
                PurchaseOrderNumber = checkout.PurchaseOrderNumber
            };
        }
    }
}