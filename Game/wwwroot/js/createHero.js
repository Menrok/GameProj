document.addEventListener("DOMContentLoaded", function () {
    const classSelect = document.getElementById("ClassSelect");

    const strengthValue = document.getElementById("StrengthValue");
    const dexterityValue = document.getElementById("DexterityValue");
    const intelligenceValue = document.getElementById("IntelligenceValue");

    const strengthBonus = document.getElementById("StrengthBonus");
    const dexterityBonus = document.getElementById("DexterityBonus");
    const intelligenceBonus = document.getElementById("IntelligenceBonus");

    if (!classSelect || !strengthValue || !dexterityValue || !intelligenceValue) {
        console.error("Nie znaleziono elementów w HTML. Sprawdź ID w widoku.");
        return;
    }

    classSelect.addEventListener("change", function () {
        const selectedClass = classSelect.value;

        strengthBonus.textContent = "";
        dexterityBonus.textContent = "";
        intelligenceBonus.textContent = "";

        const baseStrength = 10;
        const baseDexterity = 10;
        const baseIntelligence = 10;

        strengthValue.textContent = baseStrength;
        dexterityValue.textContent = baseDexterity;
        intelligenceValue.textContent = baseIntelligence;

        if (selectedClass === "Wojownik") {
            strengthBonus.textContent = "(+10)";
            strengthValue.textContent = baseStrength + 10;
        } else if (selectedClass === "Mag") {
            intelligenceBonus.textContent = "(+10)";
            intelligenceValue.textContent = baseIntelligence + 10;
        } else if (selectedClass === "Łucznik") {
            dexterityBonus.textContent = "(+10)";
            dexterityValue.textContent = baseDexterity + 10;
        }
    });
});
