namespace MusicStore.UnitTests
{
    public class SimpleTests
    {
        [Fact]
        public void CheckTwoIntegersSum()
        {
            //Arrange
            int x = 1;
            int y = 2;
            //Act
            var suma = x + y;
            var expected = 3;
            //Assert
            Assert.Equal(expected, suma);
        }
    }
}
