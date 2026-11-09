using Xunit;
using Moq;
using EncurtadorURLs.Services;
using EncurtadorURLs.Data;
using EncurtadorURLs.Models;

namespace EncurtadorURLs.Tests.Services;

/// <summary>
/// Testes unitários para UrlsService.CreateShortUrlAsync.
/// FASE RED do TDD: testes escritos ANTES da implementação existir.
/// Todos devem FALHAR neste momento — isso é o esperado e desejado.
/// </summary>
public class UrlsServiceTests
{
    private readonly Mock<IUrlContext> _mockContext;
    private readonly UrlsService _service;

    public UrlsServiceTests()
    {
        _mockContext = new Mock<IUrlContext>();
        _service = new UrlsService(_mockContext.Object);
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComUrlValida_DeveRetornarUrlItemComShortCode()
    {
        // Arrange
        var urlOriginal = "https://www.google.com";
        _mockContext.Setup(c => c.ShortCodeExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var resultado = await _service.CreateShortUrlAsync(urlOriginal);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(urlOriginal, resultado.OriginalUrl);
        Assert.False(string.IsNullOrEmpty(resultado.ShortCode));

        // Item 3: Verifica se o método AddAsync foi chamado exatamente uma vez
        _mockContext.Verify(c => c.AddAsync(It.IsAny<UrlItem>()), Times.Once);
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComUrlVazia_DeveLancarArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateShortUrlAsync(string.Empty)
        );
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComUrlNula_DeveLancarArgumentException()
    {
        // Item 1: Act & Assert para URL nula
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateShortUrlAsync(null!)
        );
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComUrlMuitoLonga_DeveLancarArgumentException()
    {
        // Item 2: Arrange com URL > 2048 caracteres
        var urlMuitoLonga = "https://exemplo.com/" + new string('a', 2100);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateShortUrlAsync(urlMuitoLonga)
        );
    }

    [Fact]
    public async Task CreateShortUrlAsync_ComEsquemaInvalido_DeveLancarArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateShortUrlAsync("ftp://arquivo-invalido.com")
        );
    }
}