namespace GLSTest

[<TestFixture>]
type TestClass() = 

    [<Test>]
    member this.When2IsAddedTo2Expect4() = 
        Assert.AreEqual(4, 2+2)