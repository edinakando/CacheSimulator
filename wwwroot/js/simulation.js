var Simulation = {
    instructionLines: [],
    instructionCount: 0,
    isDirectMapedCache: 0,
    isFullyAssociativeCache: 0,
    cacheLineSize: 0,
    currentMemoryAddress: 0,

    startSimulation: function () {
        $("#startSimulationButton").hide();
        $("#nextStepButton").attr("hidden", false);
        $("#instructionSet").attr("contentEditable", false);

        this.instructionLines = $("#instructionSet").html().split('<br>');
        this.highlightCurrentInstruction(0);

        var memorySize = $("#memorySize").val();
        var cacheSize = $("#cacheSize").val();
        var dataSize = $("#dataSize").val();

        var replacementAlgorithm = $("input[type='radio'][name='replacementAlgorithm']:checked").val();
        var writePolicy = $("input[type='radio'][name='writePolicy']:checked").val();
        var writePolicyAllocate = $("input[type='radio'][name='writePolicyAllocate']:checked").val();

        Simulation.cacheLineSize = cacheSize / dataSize;

        var operations = [];
        for (var line = 0; line < this.instructionLines.length; line++) {
            var instruction = this.instructionLines[line].split(' ');

            if (instruction[0] != "") {
                this.instructionCount++;
                operations.push({
                    'Type': instruction[0],
                    'Address': instruction[1],
                    'Data': instruction[2] === undefined ? null : instruction[2]
                })
            }
        }

        var simulationParameters = {
            'Operations': operations,
            'MemorySize': memorySize,
            'CacheSize': cacheSize,
            'DataSize': dataSize,
            'ReplacementAlgorithm': replacementAlgorithm,
            'WritePolicy': writePolicy,
            'WritePolicyAllocate': writePolicyAllocate
        };

        var windowLocation = window.location.pathname.toLowerCase();

        if (windowLocation.includes("directmapped")) {
            this.isDirectMapedCache = 1;   //sa nu ramana 1 dupa ce se schimba tab-ul -> investigate
            DirectMappedSimulation.startSimulation(simulationParameters);
        }
        else if (windowLocation.includes("fullyassociative")) {
            this.isFullyAssociativeCache = 1
            FullyAssociativeSimulation.startSimulation(simulationParameters);
        }
    },

    highlightCurrentInstruction: function (currentInstruction) {
        var newDivContent = "";

        for (var line = 0; line < this.instructionLines.length; line++) {
            if (line == currentInstruction) {
                newDivContent += "<div class=\"highlight\" style=\"width=100%\">" + this.instructionLines[line] + "</div>";
            }
            else {
                newDivContent += this.instructionLines[line] + "<br>";
            }
        }

        $("#instructionSet").html(newDivContent);
    },

    updateButtons: function () {
        $("#nextStepButton").attr("hidden", true);
        this.instructionCount--;

        if (this.instructionCount == 0) {
            $("#finishButton").attr("hidden", false);
        }
        else {
            $("#nextInstructionButton").attr("hidden", false);
        }
    },

    nextStep: function () {
        if (this.isDirectMapedCache == 1) {
            DirectMappedSimulation.nextStep();
        }
        else if (this.isFullyAssociativeCache == 1) {
            FullyAssociativeSimulation.nextStep();
        }
    },

    nextInstruction: function () {
        if (this.isDirectMapedCache == 1) {
            DirectMappedSimulation.nextInstruction();
        }
        else if (this.isFullyAssociativeCache == 1) {
            FullyAssociativeSimulation.nextInstruction();
        }
    },

    finish: function () {
        if (this.isDirectMapedCache == 1) {
            DirectMappedSimulation.finish();
        }
        else if (this.isFullyAssociativeCache == 1) {
            FullyAssociativeSimulation.finish();
        }
    }
}