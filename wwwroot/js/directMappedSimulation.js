var DirectMappedSimulation = {
    currentStep: 0,
    currentInstruction: 0,
    index: 0,
    isValid: 0,
    isHit: 0,
    currentMemoryAddress: 0,
    instructionType: "",
    memoryAddress: 0,
    operations: [],
    isCheckingDirtyBit: 0,
    isComparingTags: 0,

    startSimulation: function (simulationParameters) {
        TableCreator.drawCacheAndMemory(simulationParameters.CacheSize / simulationParameters.DataSize, 1);
        this.operations = simulationParameters.Operations;
        this.instructionType = this.operations[this.currentInstruction].Type;
        this.memoryAddress = this.operations[this.currentInstruction].Address;

        $.ajax({
            type: "POST",
            async: false,
            url: "/DirectMappedCacheSimulation/DirectMappedCache",
            data: { simulationParameters: simulationParameters },
            success: function () {
                DirectMappedSimulation.nextStep();
            },
            error: function () {
            }
        });
    },

    nextStep: function () {
        this.currentStep++;

        if (this.currentStep == 1) {
            $.notify("Adress breakdown", "success");
            this.addressBreakdown();
        }
        else if (this.currentStep == 2) {
            $.notify("Checking corresponding index", "success");
            $("#set-0-cacheRow-" + this.index).addClass('highlight');
        }
        else if (this.currentStep == 3) {
            $.notify("Checking valid bit", "success");
            $("#set-0-valid-" + this.index).addClass('highlight-more');
        }
        else if (this.currentStep == 4) {
            var valid = $("#set-0-valid-" + this.index).text();
            if (valid == 0) {
                $.notify("Valid bit is 0 => Cache Miss", "error");
                $("#set-0-valid-" + this.index).addClass('highlight-red');
            }
            else {
                $.notify("Valid bit is 1 => Compare the Tags", "success");

                $("#set-0-valid-" + this.index).removeClass('highlight-more');
                $("#tagValue").addClass('highlight-more');
                $("#tag-" + this.index).addClass('highlight-more');

                this.isComparingTags = 1;

                $.ajax({
                    type: 'GET',
                    url: '/DirectMappedCacheSimulation/ExecuteCurrentOperation',
                    success: function (response) {
                        DirectMappedSimulation.response = response;
                    }
                });
            }
        }
        else if (this.instructionType.toLowerCase() == "read") {
            if (this.currentStep == 5 && this.isComparingTags == 0) {
                $.notify("Checking dirty bit", "success");
                $("#set-0-dirty-" + this.index).addClass('highlight-more');
                this.isCheckingDirtyBit = 1;

                $("#set-0-valid-" + this.index).removeClass('highlight-red');
                $("#set-0-valid-" + this.index).removeClass('highlight-more');
            }
            else if (this.currentStep == 5 && this.isComparingTags == 1) {
                var currentMemoryTag = $("#tagValue").text();
                var currentCacheTag = $("#set-0-tag-" + this.index).text();

                if (currentMemoryTag == currentCacheTag) {
                    $.notify("Tags are equal => Cache HIT", "success");
                    $("#set-0-cacheRow-" + this.index).addClass('highlight-hit');
                    this.isHit = 1;
                }
                else {
                    $.notify("Tags are different => Cache MISS", "error");
                    $("#tagValue").addClass('highlight-red');
                    $("#set-0-tag-" + this.index).addClass('highlight-red');
                }

                this.isComparingTags = 0;
            }
            else if (this.currentStep == 6 && this.isCheckingDirtyBit) {
                DirectMappedSimulation.checkDirtyBit();
            }
            else if (this.currentStep == 6 && this.isHit == 0) {
                $.notify("Checking dirty bit", "success");
                $("#set-0-dirty-" + this.index).addClass('highlight-more');
                this.isCheckingDirtyBit = 1
            }
            else if (this.currentStep == 7 && this.isCheckingDirtyBit) {
                DirectMappedSimulation.checkDirtyBit();
            }
            else if (this.currentStep == 8) {
                DirectMappedSimulation.updateCache();
            }
            else {
                this.isHit = 0;
                Simulation.updateButtons();
            }
        }
        else if (this.instructionType.toLowerCase() == "write") {
            if (this.currentStep == 5 && this.isComparingTags == 1) {  //Compare tag
                var currentMemoryTag = $("#tagValue").text();
                var currentCacheTag = $("#set-0-tag-" + this.index).text();

                if (currentMemoryTag == currentCacheTag) {
                    $.notify("Tags are equal => Cache HIT", "success");
                    $("#set-0-cacheRow-" + this.index).addClass('highlight-hit');
                    this.isHit = 1;
                }
                else {
                    $.notify("Tags are different => Cache MISS", "error");
                    $("#tagValue").addClass('highlight-red');
                    $("#set-0-tag-" + this.index).addClass('highlight-red');
                }
            }
            else if (this.currentStep == 5 && this.isComparingTags == 0) {
                this.writeToMemory();
            }
            else if (this.currentStep == 6 && this.isComparingTags == 1 && this.isHit == 0) {
                $.notify("Checking dirty bit", "success");
                $("#set-0-dirty-" + this.index).addClass('highlight-more');
                this.isCheckingDirtyBit = 1
            }
            else if (this.currentStep == 6 && this.isHit) {
                this.writeToMemory();
            }
            else if (this.currentStep == 7 && this.isHit == 0) {
                this.checkDirtyBit();
            }
            else if (this.currentStep == 8) {
                this.writeToMemory();
            }
            else {
                 Simulation.updateButtons();
            }
        }
    },

    nextInstruction: function () {
        this.currentStep = 0;
        this.currentInstruction++;

        this.instructionType = this.operations[this.currentInstruction].Type;
        this.memoryAddress = this.operations[this.currentInstruction].Address;

        this.isHit = 0;
        this.isCheckingDirtyBit = 0;
        this.isValid = 0;

        $("#nextStepButton").attr("hidden", false);
        $("#nextInstructionButton").attr("hidden", true);

        $("#addressRow").removeAttr('class');
        $("#set-0-cacheRow-" + this.index).removeAttr('class');
        $("#set-0-valid-" + DirectMappedSimulation.index).removeAttr('class');
        $("#set-0-tag-" + DirectMappedSimulation.index).removeAttr('class');
        $("#set-0-data-" + DirectMappedSimulation.index).removeAttr('class');
        $("#set-0-dirty-" + DirectMappedSimulation.index).removeAttr('class');
        $("#tagValue").removeAttr('class');

        for (var i = Simulation.currentMemoryAddress; i < Simulation.currentMemoryAddress + Simulation.cacheLineSize; i++) {
            $("#memory-" + i).removeClass('highlight');
        }

        Simulation.highlightCurrentInstruction(this.currentInstruction);

        $.ajax({
            type: 'POST',
            url: 'DirectMappedCacheSimulation/NextInstruction'
        });
    },

    finish: function () {
        location.reload();

        $.ajax({
            type: 'POST',
            url: "/DirectMappedCacheSimulation/Reset"
        });
    },

    addressBreakdown: function () {
        $.ajax({
            type: 'GET',
            async: false,
            url: '/DirectMappedCacheSimulation/GetCurrentAddressBreakdown',
            success: function (response) {
                $("#indexValue").text(response.indexBinary);
                $("#tagValue").text(response.tagBinary);
                $("#offsetValue").text(response.offsetBinary);
                $("#addressRow").addClass('highlight');

                DirectMappedSimulation.index = parseInt(response.indexBinary, 2);
            }
        });
    },

    updateCache: function () {
        $.ajax({
            type: 'GET',
            url: 'DirectMappedCacheSimulation/UpdateCache',
            async: false,
            success: function (response) {
                $("#set-0-valid-" + DirectMappedSimulation.index).text('1');
                $("#set-0-tag-" + DirectMappedSimulation.index).text(response.tags[DirectMappedSimulation.index]);
                $("#set-0-data-" + DirectMappedSimulation.index).text(response.cacheLines[DirectMappedSimulation.index].data.toString());

                Simulation.currentMemoryAddress = response.currentMemoryAddress * Simulation.cacheLineSize;

                for (var i = Simulation.currentMemoryAddress; i < Simulation.currentMemoryAddress + Simulation.cacheLineSize; i++) {
                    $("#memory-" + i).addClass('highlight');
                }

                $("#tagValue").removeClass('highlight-red');
                $("#set-0-tag-" + DirectMappedSimulation.index).removeClass('highlight-red');

                $("#set-0-tag-" + DirectMappedSimulation.index).addClass('highlight-more');
                $("#set-0-data-" + DirectMappedSimulation.index).addClass('highlight-more');

                Simulation.updateButtons();
            }
        });
    },

    checkDirtyBit: function () {
        if ($("#set-0-dirty-" + this.index).text() == 1) {
            $.notify("Dirty bit is 1 => update memory", "success");
            DirectMappedSimulation.updateMemoryOnRead();
        }
        else {
            $.notify("Dirty bit is 0 => update cache", "success");
            $("#set-0-dirty-" + this.index).removeClass('highlight-more');
            DirectMappedSimulation.updateCache();
        }

        this.isCheckingDirtyBit = 0;
    },

    updateMemoryOnRead: function () {
        $.ajax({
            type: 'GET',
            url: 'DirectMappedCacheSimulation/UpdateMemoryOnRead',
            success: function (response) {
                DirectMappedSimulation.updateUIAfterWrite(response);
            }
        });
    },

    updateUIAfterWrite: function (response) {
        Simulation.currentMemoryAddress = response.cacheViewModel.currentMemoryAddress * Simulation.cacheLineSize;
        var memoryAddress = Simulation.currentMemoryAddress; //+ response.updatedPlaceInMemoryBlock;

        if (response.isCacheUpdated) {
            $.notify("Updating cache", "success");

            if (DirectMappedSimulation.isHit == 0) {
                $("#set-0-tag-" + DirectMappedSimulation.index).addClass('highlight-more');
            }

            $("#set-0-valid-" + DirectMappedSimulation.index).text('1');
            $("#set-0-tag-" + DirectMappedSimulation.index).text(response.cacheViewModel.tags[DirectMappedSimulation.index]);
            $("#set-0-data-" + DirectMappedSimulation.index).text(response.cacheViewModel.cacheLines[DirectMappedSimulation.index].data.toString());
            $("#set-0-dirty-" + DirectMappedSimulation.index).text(response.cacheViewModel.dirtyBit[DirectMappedSimulation.index]);

            $("#set-0-data-" + DirectMappedSimulation.index).addClass('highlight-more');
        }
        if (response.isMemoryUpdated) {
            $.notify("Updating memory", "success");

            for (var indexInMemoryBlock = 0; indexInMemoryBlock < Simulation.cacheLineSize; indexInMemoryBlock++) {
                $("#memory-" + (memoryAddress + indexInMemoryBlock)).addClass('highlight');
                $("#memory-" + (memoryAddress + indexInMemoryBlock)).text(response.memory[response.cacheViewModel.currentMemoryAddress].data[indexInMemoryBlock]);
            }

            $("#set-0-dirty-" + DirectMappedSimulation.index).text(0);
        }
    },

    writeToMemory: function () {
        $.ajax({
            type: 'GET',
            url: '/DirectMappedCacheSimulation/WriteToMemory',
            success: function (response) {
                DirectMappedSimulation.updateUIAfterWrite(response);
            }
        });
    }
} 