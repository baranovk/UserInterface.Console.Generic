namespace UserInterface.Console.Generic.Tests.Unit;

internal sealed class ArgumentsParsingTests
{
    [Test]
    public void ParsingEmptyArgsList_Should_ReturnEmptyArgsDictionary()
    {
        var args = Array.Empty<string>();
        Assert.That(args.ParseArguments().Keys, Has.Count.EqualTo(0));
    }

    [Test]
    public void ParsingPairedArguments_Should_ReturnArgsDictionaryWithEvenKeysCount()
    {
        var args = new string[] { "-aa", "aaa", "-bb", "bbb" };
        var argsDict = args.ParseArguments();

        Assert.That(argsDict.Keys, Has.Count.EqualTo(2));
        Assert.That(argsDict.ContainsKey("aa"), Is.True);
        Assert.That(argsDict.ContainsKey("bb"), Is.True);
        Assert.That(argsDict["aa"], Is.EqualTo("aaa"));
        Assert.That(argsDict["bb"], Is.EqualTo("bbb"));
    }

    [Test]
    public void ParsingArgumentsWithoutValue_Should_ReturnArgsDictionaryWithEmptyValues()
    {
        var args = new string[] { "-aa", "aaa", "-bb" };
        var argsDict = args.ParseArguments();

        Assert.That(argsDict.Keys, Has.Count.EqualTo(2));
        Assert.That(argsDict.ContainsKey("aa"), Is.True);
        Assert.That(argsDict.ContainsKey("bb"), Is.True);
        Assert.That(argsDict["aa"], Is.EqualTo("aaa"));
        Assert.That(argsDict["bb"], Is.EqualTo(string.Empty));
    }

    [Test]
    public void ParsingArgumentsWithMultiWordValue_Should_ReturnArgsDictionaryWithJoinedWordsValues()
    {
        var args = new string[] { "-aa", "aaa1 aaa2", "-bb", "bbb1 bbb2 bbb3", "-cc" };
        var argsDict = args.ParseArguments();

        Assert.That(argsDict.Keys, Has.Count.EqualTo(3));
        Assert.That(argsDict.ContainsKey("aa"), Is.True);
        Assert.That(argsDict.ContainsKey("bb"), Is.True);
        Assert.That(argsDict.ContainsKey("cc"), Is.True);
        Assert.That(argsDict["aa"], Is.EqualTo("aaa1 aaa2"));
        Assert.That(argsDict["bb"], Is.EqualTo("bbb1 bbb2 bbb3"));
        Assert.That(argsDict["cc"], Is.EqualTo(string.Empty));
    }
}
