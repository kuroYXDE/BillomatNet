﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Develappers.BillomatNet.Api;
using Develappers.BillomatNet.Types;
using Invoice = Develappers.BillomatNet.Types.Invoice;
using InvoiceItem = Develappers.BillomatNet.Types.InvoiceItem;
using InvoiceDocument = Develappers.BillomatNet.Types.InvoiceDocument;
using InvoiceTax = Develappers.BillomatNet.Types.InvoiceTax;

namespace Develappers.BillomatNet.Helpers
{
    internal static class InvoiceMappingExtensions
    {
        internal static Invoice ToDomain(this InvoiceWrapper value)
        {
            return value?.Invoice.ToDomain();
        }

        internal static InvoiceItem ToDomain(this InvoiceItemWrapper value)
        {
            return value?.InvoiceItem.ToDomain();
        }

        internal static InvoiceDocument ToDomain(this InvoiceDocumentWrapper value)
        {
            return value?.Pdf.ToDomain();
        }

        private static InvoiceDocument ToDomain(this Api.InvoiceDocument value)
        {
            if (value == null)
            {
                return null;
            }

            return new InvoiceDocument
            {
                Id = int.Parse(value.Id),
                Created = DateTime.Parse(value.Created, CultureInfo.InvariantCulture),
                FileName = value.FileName,
                FileSize = int.Parse(value.FileSize, CultureInfo.InvariantCulture),
                InvoiceId = int.Parse(value.InvoiceId, CultureInfo.InvariantCulture),
                MimeType = value.MimeType,
                Bytes = Convert.FromBase64String(value.Base64File)
            };
        }

        internal static Types.PagedList<Invoice> ToDomain(this InvoiceListWrapper value)
        {
            return value?.Item.ToDomain();
        }

        internal static Types.PagedList<InvoiceItem> ToDomain(this InvoiceItemListWrapper value)
        {
            return value?.Item.ToDomain();
        }

        internal static Types.PagedList<Invoice> ToDomain(this InvoiceList value)
        {
            if (value == null)
            {
                return null;
            }

            return new Types.PagedList<Invoice>
            {
                Page = value.Page,
                ItemsPerPage = value.PerPage,
                TotalItems = value.Total,
                List = value.List?.Select(ToDomain).ToList()
            };
        }

        internal static Types.PagedList<InvoiceItem> ToDomain(this InvoiceItemList value)
        {
            if (value == null)
            {
                return null;
            }

            return new Types.PagedList<InvoiceItem>
            {
                Page = value.Page,
                ItemsPerPage = value.PerPage,
                TotalItems = value.Total,
                List = value.List?.Select(ToDomain).ToList()
            };
        }

        private static Invoice ToDomain(this Api.Invoice value)
        {
            if (value == null)
            {
                return null;
            }

            NetGrossType netGrossType;
            switch (value.NetGross.ToLowerInvariant())
            {
                case "net":
                    netGrossType = NetGrossType.Net;
                    break;
                case "gross":
                    netGrossType = NetGrossType.Gross;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SupplyDateType? supplyDateType;
            ISupplyDate supplyDate;
            switch (value.SupplyDateType.ToLowerInvariant())
            {
                case "supply_date":
                    supplyDateType = SupplyDateType.SupplyDate;
                    supplyDate = new DateSupplyDate
                    {
                        Date = value.SupplyDate.ToOptionalDateTime()
                    };
                    break;
                case "delivery_date":
                    supplyDateType = SupplyDateType.DeliveryDate;
                    supplyDate = new DateSupplyDate
                    {
                        Date = value.SupplyDate.ToOptionalDateTime()
                    };
                    break;
                case "supply_text":
                    supplyDateType = SupplyDateType.SupplyDate;
                    supplyDate = new FreeTextSupplyDate
                    {
                        Text = value.SupplyDate
                    };
                    break;
                case "delivery_text":
                    supplyDateType = SupplyDateType.DeliveryDate;
                    supplyDate = new FreeTextSupplyDate
                    {
                        Text = value.SupplyDate
                    };
                    break;
                case "":
                    supplyDateType = null;
                    supplyDate = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            InvoiceStatus status;
            switch (value.Status.ToLowerInvariant())
            {
                case "draft":
                    status = InvoiceStatus.Draft;
                    break;
                case "open":
                    status = InvoiceStatus.Open;
                    break;
                case "overdue":
                    status = InvoiceStatus.Overdue;
                    break;
                case "paid":
                    status = InvoiceStatus.Paid;
                    break;
                case "canceled":
                    status = InvoiceStatus.Canceled;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            IReduction reduction = null;
            if (!string.IsNullOrEmpty(value.Reduction))
            {
                if (value.Reduction.EndsWith("%"))
                {
                    reduction = new PercentReduction
                    {
                        Value = float.Parse(value.Reduction.Replace("%", ""), CultureInfo.InvariantCulture)
                    };
                }
                else
                {
                    reduction = new AbsoluteReduction
                    {
                        Value = float.Parse(value.Reduction, CultureInfo.InvariantCulture)
                    };
                }
            }

            return new Invoice
            {
                Id = int.Parse(value.Id, CultureInfo.InvariantCulture),
                InvoiceId = value.InvoiceId.ToOptionalInt(),
                ConfirmationId = value.ConfirmationId.ToOptionalInt(),
                OfferId = value.OfferId.ToOptionalInt(),
                RecurringId = value.RecurringId.ToOptionalInt(),
                TemplateId = value.TemplateId.ToOptionalInt(),
                CustomerPortalUrl = value.CustomerPortalUrl,
                ClientId = int.Parse(value.ClientId, CultureInfo.InvariantCulture),
                ContactId = value.ContactId.ToOptionalInt(),
                InvoiceNumber = value.InvoiceNumber,
                Number = value.Number.ToOptionalInt(),
                NumberPre = value.NumberPre,
                NumberLength = int.Parse(value.NumberLength, CultureInfo.InvariantCulture),
                Title = value.Title,
                Date = DateTime.Parse(value.Date, CultureInfo.InvariantCulture),
                Address = value.Address,
                Label = value.Label,
                Intro = value.Intro,
                Note = value.Note,
                TotalGross = float.Parse(value.TotalGross, CultureInfo.InvariantCulture),
                TotalNet = float.Parse(value.TotalNet, CultureInfo.InvariantCulture),
                CurrencyCode = value.CurrencyCode,
                TotalGrossUnreduced = float.Parse(value.TotalGrossUnreduced, CultureInfo.InvariantCulture),
                TotalNetUnreduced = float.Parse(value.TotalNetUnreduced, CultureInfo.InvariantCulture),
                Created = DateTime.Parse(value.Created, CultureInfo.InvariantCulture),
                DueDate = DateTime.Parse(value.DueDate, CultureInfo.InvariantCulture),
                DueDays = int.Parse(value.DueDays, CultureInfo.InvariantCulture),
                NetGross = netGrossType,
                SupplyDate = supplyDate,
                SupplyDateType = supplyDateType,
                Status = status,
                PaymentTypes = value.PaymentTypes.ToStringList(),
                Taxes = value.Taxes.ToDomain(),
                Quote = float.Parse(value.Quote, CultureInfo.InvariantCulture),
                Reduction = reduction,
                DiscountRate = float.Parse(value.DiscountRate, CultureInfo.InvariantCulture),
                DiscountDate = value.DiscountDate.ToOptionalDateTime(),
                DiscountDays = value.DiscountDays.ToOptionalInt(),
                DiscountAmount = value.DiscountAmount.ToOptionalFloat(),
                PaidAmount = value.PaidAmount.ToOptionalFloat() ?? 0,
                OpenAmount = float.Parse(value.OpenAmount, CultureInfo.InvariantCulture)
            };

        }

        private static InvoiceItem ToDomain(this Api.InvoiceItem value)
        {
            if (value == null)
            {
                return null;
            }

            IReduction reduction = null;
            if (!string.IsNullOrEmpty(value.Reduction) && value.Reduction != "null")
            {
                if (value.Reduction.EndsWith("%"))
                {
                    reduction = new PercentReduction
                    {
                        Value = float.Parse(value.Reduction.Replace("%", ""), CultureInfo.InvariantCulture)
                    };
                }
                else
                {
                    reduction = new AbsoluteReduction
                    {
                        Value = float.Parse(value.Reduction, CultureInfo.InvariantCulture)
                    };
                }
            }

            return new InvoiceItem
            {
                Id = int.Parse(value.Id, CultureInfo.InvariantCulture),
                Reduction = reduction,
                InvoiceId = int.Parse(value.InvoiceId, CultureInfo.InvariantCulture),
                ArticleId = value.ArticleId.ToOptionalInt(),
                Description = value.Description,
                Position = int.Parse(value.Position, CultureInfo.InvariantCulture),
                Title = value.Title,
                Unit = value.Unit,
                TaxName = value.TaxName,
                TotalNet = float.Parse(value.TotalNet, CultureInfo.InvariantCulture),
                Quantity = float.Parse(value.Quantity, CultureInfo.InvariantCulture),
                TotalNetUnreduced = float.Parse(value.TotalNetUnreduced, CultureInfo.InvariantCulture),
                TotalGross = float.Parse(value.TotalGross, CultureInfo.InvariantCulture),
                TotalGrossUnreduced = float.Parse(value.TotalGrossUnreduced, CultureInfo.InvariantCulture),
                UnitPrice = float.Parse(value.UnitPrice, CultureInfo.InvariantCulture),
                TaxRate = value.TaxRate.ToOptionalFloat()
            };

        }

        private static List<InvoiceTax> ToDomain(this InvoiceTaxWrapper value)
        {
            return value?.List?.Select(ToDomain).ToList();
        }

        private static InvoiceTax ToDomain(this Api.InvoiceTax value)
        {
            if (value == null)
            {
                return null;
            }

            return new InvoiceTax
            {
                Name = value.Name,
                Amount = float.Parse(value.Amount, CultureInfo.InvariantCulture),
                AmountGross = float.Parse(value.AmountGross, CultureInfo.InvariantCulture),
                AmountGrossPlain = float.Parse(value.AmountGrossPlain, CultureInfo.InvariantCulture),
                AmountGrossRounded = float.Parse(value.AmountGrossRounded, CultureInfo.InvariantCulture),
                AmountNet = float.Parse(value.AmountNet, CultureInfo.InvariantCulture),
                AmountNetPlain = float.Parse(value.AmountNetPlain, CultureInfo.InvariantCulture),
                AmountNetRounded = float.Parse(value.AmountNetRounded, CultureInfo.InvariantCulture),
                AmountPlain = float.Parse(value.AmountPlain, CultureInfo.InvariantCulture),
                AmountRounded = float.Parse(value.AmountRounded, CultureInfo.InvariantCulture),
                Rate = float.Parse(value.Rate, CultureInfo.InvariantCulture),
            };
        }

        internal static Api.Invoice ToApi(this Invoice value)
        {
            if (value == null)
                return null;

            string netGrossType;
            switch (value.NetGross)
            {
                case NetGrossType.Net:
                    netGrossType = "NET";
                    break;
                case NetGrossType.Gross:
                    netGrossType = "GROSS";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string supplyDateType;
            switch (value.SupplyDateType)
            {
                case SupplyDateType.SupplyDate:
                    supplyDateType = "supply_date";
                    break;
                case SupplyDateType.DeliveryDate:
                    supplyDateType = "delivery_date";
                    break;
                case null:
                    supplyDateType = "";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string status;
            switch (value.Status)
            {
                case InvoiceStatus.Draft:
                    status = "draft";
                    break;
                case InvoiceStatus.Open:
                    status = "open";
                    break;
                case InvoiceStatus.Overdue:
                    status = "overdue";
                    break;
                case InvoiceStatus.Paid:
                    status = "paid";
                    break;
                case InvoiceStatus.Canceled:
                    status = "canceled";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // TODO: extract
            var reduction = "";
            switch (value.Reduction)
            {
                case null:
                    reduction = "0";
                    break;
                case PercentReduction percentReduction:
                    reduction = $"{percentReduction.Value.ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case AbsoluteReduction absoluteReduction:
                    reduction = absoluteReduction.Value.ToString(CultureInfo.InvariantCulture);
                    break;
            }

            //Finds out and converts the ISupplyDate to its class and converts it to string if needed
            var strSupplyDate = "";
            switch (value.SupplyDate)
            {
                case null:
                    strSupplyDate = "";
                    break;
                case DateSupplyDate dateSupplyDate:
                    strSupplyDate = dateSupplyDate.Date.ToApiDate();
                    break;
                case FreeTextSupplyDate freeTextSupplyDate:
                    strSupplyDate = freeTextSupplyDate.Text;
                    break;
            }

            var paymentTypes = string.Join(",", value.PaymentTypes);

            if (value.InvoiceItems == null)
            {
                throw new ArgumentException();
            }

            var apiInvoiceItems = value.InvoiceItems.Select(x => x.ToApi()).ToList();

            var itemsWrapper = new InvoiceItemsWrapper
            {
                List = apiInvoiceItems
            };

            return new Api.Invoice
            {
                Id = value.Id.ToString(),
                Created = value.Created.ToApiDate(),
                ContactId = value.ContactId.ToString(),
                ClientId = value.ClientId.ToString(),
                InvoiceNumber = value.InvoiceNumber,
                Number = value.Number.ToString(),
                NumberPre = value.NumberPre,
                NumberLength = value.NumberLength.ToString(),
                Title = value.Title,
                Date = value.Date.ToApiDate(),
                SupplyDate = strSupplyDate,
                SupplyDateType = supplyDateType,
                DueDate = value.DueDate.ToApiDate(),
                DueDays = value.DueDays.ToString(),
                Address = value.Address,
                Status = status,
                DiscountRate = value.DiscountRate.ToString(CultureInfo.InvariantCulture),
                DiscountDate = value.DiscountDate.ToApiDate(),
                DiscountDays = value.DiscountDays.ToString(),
                DiscountAmount = value.DiscountAmount.ToString(),
                Label = value.Label,
                Intro = value.Intro,
                Note = value.Note,
                TotalGross = value.TotalGross.ToString(CultureInfo.InvariantCulture),
                TotalNet = value.TotalNet.ToString(CultureInfo.InvariantCulture),
                CurrencyCode = value.CurrencyCode,
                Quote = value.Quote.ToString(CultureInfo.InvariantCulture),
                NetGross = netGrossType,
                Reduction = reduction,
                TotalGrossUnreduced = value.TotalGrossUnreduced.ToString(CultureInfo.InvariantCulture),
                TotalNetUnreduced = value.TotalNetUnreduced.ToString(CultureInfo.InvariantCulture),
                PaidAmount = value.PaidAmount.ToString(CultureInfo.InvariantCulture),
                OpenAmount = value.OpenAmount.ToString(CultureInfo.InvariantCulture),
                CustomerPortalUrl = value.CustomerPortalUrl,
                InvoiceId = value.InvoiceId.ToString(),
                OfferId = value.OfferId.ToString(),
                ConfirmationId = value.ConfirmationId.ToString(),
                RecurringId = value.RecurringId.ToString(),
                TemplateId = value.TemplateId.ToString(),
                PaymentTypes = paymentTypes,
                Taxes = new InvoiceTaxWrapper(),
                InvoiceItems = itemsWrapper
            };
        }

        internal static Api.InvoiceItem ToApi(this InvoiceItem value)
        {
            if (value == null)
            {
                return null;
            }

            // TODO: extract
            var reduction = "";
            switch (value.Reduction)
            {
                case null:
                    reduction = "0";
                    break;
                case PercentReduction percentReduction:
                    reduction = $"{percentReduction.Value.ToString(CultureInfo.InvariantCulture)}%";
                    break;
                case AbsoluteReduction absoluteReduction:
                    reduction = absoluteReduction.Value.ToString(CultureInfo.InvariantCulture);
                    break;
            }

            return new Api.InvoiceItem
            {
                ArticleId = value.ArticleId.ToString(),
                InvoiceId = value.InvoiceId.ToString(),
                Position = value.Position.ToString(),
                Unit = value.Unit,
                Quantity = value.Quantity.ToString(CultureInfo.InvariantCulture),
                UnitPrice = value.UnitPrice.ToString(CultureInfo.InvariantCulture),
                TaxName = value.TaxName,
                TaxRate = value.TaxRate.ToString(),
                Title = value.Title,
                Description = value.Description,
                TotalGross = value.TotalGross.ToString(CultureInfo.InvariantCulture),
                TotalNet = value.TotalNet.ToString(CultureInfo.InvariantCulture),
                Reduction = reduction,
                TotalGrossUnreduced = value.TotalGrossUnreduced.ToString(CultureInfo.InvariantCulture),
                TotalNetUnreduced = value.TotalNetUnreduced.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}