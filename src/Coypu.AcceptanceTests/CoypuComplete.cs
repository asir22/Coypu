﻿using NUnit.Framework;

namespace Coypu.AcceptanceTests
{
    [TestFixture, Explicit]
    public class CoypuComplete
    {

        [TearDown]
        public void TearDown()
        {
            Browser.EndSession();
        }

        [Test]
        public void Retries_Autotrader()
        {
            var browser = Browser.Session;
            browser.Visit("http://www.autotrader.co.uk/used-cars");

            browser.FillIn("postcode").With("N1 1AA");

            browser.Select("citroen").From("make");
            browser.Select("c4_grand_picasso").From("model");

            browser.Select("National").From("radius");
            browser.Select("diesel").From("fuel-type");
            browser.Select("up_to_7_years_old").From("maximum-age");
            browser.Select("up_to_60000_miles").From("maximum-mileage");

            browser.FillIn("Add keyword").With("vtr");
        }


        [Test]
        public void Visibility_NewTwitterLogin()
        {
            var browser = Browser.Session;
            browser.Visit("http://www.twitter.com");

            browser.FillIn("session[username_or_email]").With("coyputester2");
            browser.FillIn("session[password]").With("Nappybara");

            browser.ClickButton("Sign in");
        }


        [Test]
        public void FindingStuff_CarBuzz()
        {
            var browser = Browser.Session;
            browser.Visit("http://carbuzz.heroku.com/car_search");

            browser.WithinSection("Make",
                                  () =>
                                  {
                                      browser.Check("Audi");
                                      browser.Check("BMW");
                                      browser.Check("Mercedes");
                                  });

            Assert.That(browser.HasContent(@"\b5 car reviews found"));

            browser.WithinSection("Seats", () => browser.ClickButton("4"));

            Assert.That(browser.HasContent(@"\b2 car reviews found"));

        }
    }
}