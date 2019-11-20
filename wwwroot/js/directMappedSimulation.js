var DirectMappedSimulation = {
    currentStep: 0,
    currentInstruction: 0,
    index: 0,
    isValid: 0,
    isHit: 0,
    currentMemoryAddress: 0,

    startSimulation: function (simulationParameters) {
       // CacheTableCreator.drawDirectMappedCache();

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
            $("#cacheRow-" + this.index).addClass('highlight');
        }
        else if (this.currentStep == 3) {
            $.notify("Checking valid bit", "success");
            $("#valid-" + this.index).addClass('highlight-more');
        }
        else if (this.currentStep == 4) {
            var valid = $("#valid-" + this.index).text();
            if (valid == 0) {
                $.notify("Valid bit is 0 => Cache Miss", "error");
                $("#valid-" + this.index).addClass('highlight-red');
            }
            else {
                $.notify("Valid bit is 1 => Compare the Tags", "success");

                $("#valid-" + this.index).removeClass('highlight-more');
                $("#tagValue").addClass('highlight-more');
                $("#tag-" + this.index).addClass('highlight-more');

                this.isValid = 1;
            }
        }
        else if (this.currentStep == 5 && this.isValid == 0) {
            $.notify("Updating cache", "success");
            $("#valid-" + this.index).removeClass('highlight-red');
            $("#valid-" + this.index).removeClass('highlight-more');

            DirectMappedSimulation.updateCache();
        }
        else if (this.currentStep == 5 && this.isValid == 1) {  //Compare tag
            var currentMemoryTag = $("#tagValue").text();
            var currentCacheTag = $("#tag-" + this.index).text();

            if (currentMemoryTag == currentCacheTag) {
                $.notify("Tags are equal => Cache HIT", "success");
                this.isHit = 1;
            }
            else {
                $.notify("Tags are different => Cache MISS", "error");
                $("#tagValue").addClass('highlight-red');
                $("#tag-" + this.index).addClass('highlight-red');
            }

            this.isValid = 0;
        }
        else if (this.currentStep == 6 && this.isHit == 0) {
            $.notify("Updating cache", "success");

            DirectMappedSimulation.updateCache();
        }
        else {
            this.isHit = 0;
            Simulation.updateButtons();
        }
    },

    nextInstruction: function () {
        this.currentStep = 0;
        this.currentInstruction++;

        $("#nextStepButton").attr("hidden", false);
        $("#nextInstructionButton").attr("hidden", true);

        $("#addressRow").removeAttr('class');
        $("#cacheRow-" + this.index).removeAttr('class');
        $("#valid-" + DirectMappedSimulation.index).removeAttr('class');
        $("#tag-" + DirectMappedSimulation.index).removeAttr('class');
        $("#data-" + DirectMappedSimulation.index).removeAttr('class');
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
                $("#valid-" + DirectMappedSimulation.index).text('1');
                $("#tag-" + DirectMappedSimulation.index).text(response.tags[DirectMappedSimulation.index]);
                $("#data-" + DirectMappedSimulation.index).text(response.cacheLines[DirectMappedSimulation.index].data.toString());

                Simulation.currentMemoryAddress = response.currentMemoryAddress * Simulation.cacheLineSize;

                for (var i = Simulation.currentMemoryAddress; i < Simulation.currentMemoryAddress + Simulation.cacheLineSize; i++) {
                    $("#memory-" + i).addClass('highlight');
                }

                $("#tagValue").removeClass('highlight-red');
                $("#tag-" + DirectMappedSimulation.index).removeClass('highlight-red');

                $("#tag-" + DirectMappedSimulation.index).addClass('highlight-more');
                $("#data-" + DirectMappedSimulation.index).addClass('highlight-more');

                Simulation.updateButtons();
            }
        });
    },

} 