﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using FluentAssertions;

namespace Markdown.tests
{
    [TestFixture]
    public class Markdown_Should
    {
        [TestCase("abc")]
        [TestCase("abc def ghi")]
        [TestCase("Hello, world!")]
        public void TextWithoutSpecialTags_Should_NotChanged(string inputText)
        {
            Md.Render(inputText).Should().Be(inputText);
        }

        [TestCase("_abc")]
        [TestCase("__abc def ghi")]
        [TestCase("__Hello, _world!")]
        public void TextWithSingleTags_Should_NotChange(string inputText)
        {
            Md.Render(inputText).Should().Be(inputText);
        }

        [TestCase("_abc_", ExpectedResult = "<em>abc</em>")]
        [TestCase("_abcdef_ ghi", ExpectedResult = "<em>abcdef</em> ghi")]
        [TestCase("Hello, _world_!", ExpectedResult = "Hello, <em>world</em>!")]
        public string LineBetweenUnderline_Should_BeEqualExcepted(string inputText)
        {
            return Md.Render(inputText);
        }

        [TestCase("__abc__", ExpectedResult = "<strong>abc</strong>")]
        [TestCase("__abcdef__ ghi", ExpectedResult = "<strong>abcdef</strong> ghi")]
        [TestCase("Hello, __world__!", ExpectedResult = "Hello, <strong>world</strong>!")]
        public string LineBetweenTwoUnderline_Should_BeEqualExcepted(string inputText)
        {
            return Md.Render(inputText);
        }

        [TestCase("##abc", ExpectedResult = "<h2>abc</h2>")]
        [TestCase("####abcdef", ExpectedResult = "<h4>abcdef</h4>")]
        [TestCase("#abcdef#", ExpectedResult = "<h1>abcdef</h1>")]
        [TestCase("##abcdef##", ExpectedResult = "<h2>abcdef</h2>")]
        public string LineAfterLattice_Should_BeEqualExcepted(string inputText)
        {
            return Md.Render(inputText);
        }

        [Test]
        public void LineAfterLaticeBeforeNewLine_Should_Change()
        {
            var input = @"##Заголовок
Текст";
            Md.Render(input).Should().BeEquivalentTo(@"<h2>Заголовок</h2>Текст");
        }

        [TestCase("__abcdef__ _ghi_", ExpectedResult = "<strong>abcdef</strong> <em>ghi</em>")]
        [TestCase("_Hello_, __world__!", ExpectedResult = "<em>Hello</em>, <strong>world</strong>!")]
        public string AllLineBetweenUnderline_Should_Change(string inputText)
        {
            return Md.Render(inputText);
        }

        [TestCase("_a__b__c_", ExpectedResult = "<em>a__b__c</em>")]
        [TestCase("_a__bcde__f_ ghi", ExpectedResult = "<em>a__bcde__f</em> ghi")]
        [TestCase("Hello, _wo__r__ld_!", ExpectedResult = "Hello, <em>wo__r__ld</em>!")]
        [TestCase("__a_b__c_", ExpectedResult = "__a<em>b__c</em>")]
        public string DoubleUnderline_Should_NotChange(string inputText)
        {
            return Md.Render(inputText);
        }

        [TestCase("__a_b_c__", ExpectedResult = "<strong>a<em>b</em>c</strong>")]
        [TestCase("__a_bcde_f__ ghi", ExpectedResult = "<strong>a<em>bcde</em>f</strong> ghi")]
        [TestCase("Hello, __wo_r_ld__!", ExpectedResult = "Hello, <strong>wo<em>r</em>ld</strong>!")]
        [TestCase("H__ell_o, __wor_ld__!", ExpectedResult = "H<strong>ell<em>o, __wor</em>ld</strong>!")]
        public string SingleUnderline_Should_Change(string inputText)
        {
            return Md.Render(inputText);
        }

        [TestCase("\\abc", ExpectedResult = "abc")]
        [TestCase("abc \\def ghi", ExpectedResult = "abc def ghi")]
        [TestCase("Hello, _world\\!", ExpectedResult = "Hello, _world!")]
        public string EscapeSymbols_Should_BeRemoved(string inputText)
        {
            return Md.Render(inputText);
        }

        [TestCase("\\_abc_", ExpectedResult = "_abc_")]
        [TestCase("abc \\_\\_def__ ghi", ExpectedResult = "abc __def__ ghi")]
        [TestCase("Hello, _world\\_!", ExpectedResult = "Hello, _world_!")]
        public string UnderlineWithEscapeSymbols_Should_NotChange(string inputString)
        {
            return Md.Render(inputString);
        }

        [Test]
        public void Should_WorkWithLinearTime()
        {
            var inputString = "__The__ _quick_ \\__brown_ \\_fox_  __jumps _over_ the__ _lazy __lazy__ dog_. ";
            var sb = new StringBuilder(inputString);
            for (var i = 0; i < 100; i++)
            {
                sb.Append(inputString);
            }
            var testString = sb.ToString();
            var timePreviousWork = 0;
            var sw = Stopwatch.StartNew();
            Md.Render(testString);
            sw.Stop();
            var nominalTimeWork = (int)sw.ElapsedMilliseconds; 
            for (var i = 0; i < 500; i++)
            {
                sb.Append(inputString);
                testString = sb.ToString();
                sw = Stopwatch.StartNew();
                Md.Render(testString);
                sw.Stop();
                var timeWork = (int)sw.ElapsedMilliseconds;
                (timeWork - timePreviousWork).Should().BeLessThan(nominalTimeWork * 2);
                timePreviousWork = (int)sw.ElapsedMilliseconds;
            }
        }
    }
}