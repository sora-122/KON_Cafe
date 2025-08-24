using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SampleScriptEditTest
{

    [Test]
    public void SampleScriptEditTest_Add1()
    {
        SampleScript test = new SampleScript();
        Assert.That(test.Add(100, 200), Is.EqualTo(300));
    }

    [Test]
    public void SampleScriptEditTest_Sub1()
    {
        SampleScript test = new SampleScript();
        Assert.That(test.Sub(5, 2), Is.EqualTo(3));
    }

    [Test]
    public void SampleScriptEditTest_Multi1()
    {
        SampleScript test = new SampleScript();
        Assert.That(test.Multi(3, 2), Is.EqualTo(6));
    }
}