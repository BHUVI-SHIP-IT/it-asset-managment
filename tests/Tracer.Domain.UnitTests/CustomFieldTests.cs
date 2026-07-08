using FluentAssertions;
using Tracer.Domain.Aggregates.CustomFieldAggregate;
using Xunit;

namespace Tracer.Domain.UnitTests;

/// <summary>
/// Tests for CustomField and CustomFieldValue aggregate invariants (M6, Doc 4 §3.20–3.21).
/// </summary>
public sealed class CustomFieldTests
{
    private static readonly Guid _companyId = Guid.NewGuid();

    // ── CustomField.Create ──────────────────────────────────────────────────

    [Fact]
    public void Create_TextField_WithValidData_SetsProperties()
    {
        var field = CustomField.Create(_companyId, "Serial Number", CustomFieldType.Text, isRequired: true);

        field.CompanyId.Should().Be(_companyId);
        field.Name.Should().Be("Serial Number");
        field.FieldType.Should().Be(CustomFieldType.Text);
        field.IsRequired.Should().BeTrue();
        field.Options.Should().BeNull();
    }

    [Fact]
    public void Create_TrimsName()
    {
        var field = CustomField.Create(_companyId, "  Warranty Date  ", CustomFieldType.Date, false);

        field.Name.Should().Be("Warranty Date");
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ThrowsArgumentException()
    {
        var act = () => CustomField.Create(Guid.Empty, "Name", CustomFieldType.Text, false);

        act.Should().Throw<ArgumentException>().WithParameterName("companyId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ThrowsArgumentException(string? name)
    {
        var act = () => CustomField.Create(_companyId, name!, CustomFieldType.Text, false);

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Create_DropdownWithoutOptions_ThrowsArgumentException()
    {
        var act = () => CustomField.Create(_companyId, "Status", CustomFieldType.Dropdown, false, options: null);

        act.Should().Throw<ArgumentException>().WithParameterName("options");
    }

    [Fact]
    public void Create_DropdownWithOptions_SetsOptions()
    {
        var options = "[\"Active\",\"Inactive\"]";
        var field = CustomField.Create(_companyId, "Status", CustomFieldType.Dropdown, false, options);

        field.Options.Should().Be(options);
    }

    // ── CustomField.Update ──────────────────────────────────────────────────

    [Fact]
    public void Update_ChangesAllProperties()
    {
        var field = CustomField.Create(_companyId, "Old Name", CustomFieldType.Text, false);

        field.Update("New Name", CustomFieldType.Number, true, null);

        field.Name.Should().Be("New Name");
        field.FieldType.Should().Be(CustomFieldType.Number);
        field.IsRequired.Should().BeTrue();
    }

    [Fact]
    public void Update_DropdownWithoutOptions_ThrowsArgumentException()
    {
        var field = CustomField.Create(_companyId, "My Field", CustomFieldType.Text, false);

        var act = () => field.Update("My Field", CustomFieldType.Dropdown, false, null);

        act.Should().Throw<ArgumentException>().WithParameterName("options");
    }

    // ── CustomFieldValue ────────────────────────────────────────────────────

    [Fact]
    public void CustomFieldValue_Create_WithValidData_SetsProperties()
    {
        var fieldId = Guid.NewGuid();
        var entityId = Guid.NewGuid();

        var value = CustomFieldValue.Create(fieldId, entityId, "SN-12345");

        value.CustomFieldId.Should().Be(fieldId);
        value.EntityId.Should().Be(entityId);
        value.Value.Should().Be("SN-12345");
    }

    [Fact]
    public void CustomFieldValue_Create_WithEmptyCustomFieldId_Throws()
    {
        var act = () => CustomFieldValue.Create(Guid.Empty, Guid.NewGuid(), "value");

        act.Should().Throw<ArgumentException>().WithParameterName("customFieldId");
    }

    [Fact]
    public void CustomFieldValue_Create_WithEmptyEntityId_Throws()
    {
        var act = () => CustomFieldValue.Create(Guid.NewGuid(), Guid.Empty, "value");

        act.Should().Throw<ArgumentException>().WithParameterName("entityId");
    }

    [Fact]
    public void CustomFieldValue_SetValue_UpdatesValue()
    {
        var value = CustomFieldValue.Create(Guid.NewGuid(), Guid.NewGuid(), "old");

        value.SetValue("new");

        value.Value.Should().Be("new");
    }

    [Fact]
    public void CustomFieldValue_SetValue_ToNull_ClearsValue()
    {
        var value = CustomFieldValue.Create(Guid.NewGuid(), Guid.NewGuid(), "has-value");

        value.SetValue(null);

        value.Value.Should().BeNull();
    }
}
