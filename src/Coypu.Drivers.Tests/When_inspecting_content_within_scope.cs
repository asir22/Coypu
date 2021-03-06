﻿using System;
using NSpec;

namespace Coypu.Drivers.Tests
{
    internal class When_inspecting_content_within_scope : DriverSpecs
    {
        internal override void Specs()
        {
            describe["scope 1"] = () =>
            {
                  before = () => driver.SetScope(() => driver.FindId("scope1"));

                it["finds content within scope"] = () =>
                {
                    driver.HasContent("Scope 1").should_be_true();
                };
                it["does not find content outside scope"] = () =>
                {
                    driver.HasContent("Scope 2").should_be_false();
                };
            };
            describe["scope 2"] = () =>
            {
                before = () => driver.SetScope(() => driver.FindId("scope2"));

                it["finds content within scope"] = () =>
                {
                    driver.HasContent("Scope 2").should_be_true();
                };
                it["does not find content outside scope"] = () =>
                {
                    driver.HasContent("Scope 1").should_be_false();
                };
            };
        }
    }
}