﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using CurrencyCalc.Models;
using CurrencyCalc.Utilities;
using EF.Entities;
using FirstFloor.ModernUI.Presentation;

namespace CurrencyCalc.Infrastructure
{
    public static class ExtensionMethods
    {
        private static readonly Regex Regex = new Regex(@"[?|&](\w+)=([^?|^&]+)");

        public static IReadOnlyDictionary<string, string> ParseQueryString(this Uri uri)
        {
            var match = Regex.Match(uri.OriginalString);
            var paramaters = new Dictionary<string, string>();
            while (match.Success)
            {
                paramaters.Add(match.Groups[1].Value, match.Groups[2].Value);
                match = match.NextMatch();
            }
            return paramaters;
        }

        public static LinkCollection ToLinksCollection(this IEnumerable<CurrencyEF> currencies)
        {
            var toReturn = new LinkCollection();

            foreach (var currency in currencies)
            {
                toReturn.Add(currency.ToLink());
            }

            return toReturn;
        }

        public static Link ToLink(this CurrencyEF currency)
        {
            return new Link
            {
                DisplayName = currency.Name,
                Source = new Uri(
                    "/Content/SelectedCurrencyHistory.xaml?Name=" + currency.Name,
                    UriKind.Relative)
            };
        }

        public static IEnumerable<string> ToListOfString(this IEnumerable<CurrencyEF> currencies)
        {
            return currencies.Select(x => x.Name);
        }

        public static void Update(this IEnumerable<CurrencyEF> currencies, IEnumerable<rate> newCurrencies)
        {
            foreach (var currency in currencies)
            {
                var found = newCurrencies.First(x => x.Id.Substring(0, 3) == currency.Name);
                currency.CurrentValue = found.Rate.MapTheDouble();
                currency.LastUpdate = Mappers.MapTheDate(found.Date, found.Time);
            }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> original)
        {
            return new ObservableCollection<T>(original);
        }
    }
}
