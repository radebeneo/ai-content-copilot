(async function () {
    console.log("AI Copilot initializing...");

    async function loadCopilot() {
        try {
            const response = await fetch('/api/ai-copilot/render');
            if (!response.ok) {
                console.error("Failed to fetch AI Copilot partial view");
                return;
            }

            const html = await response.text();
            
            // Create a container for the copilot
            const container = document.createElement('div');
            container.id = 'ai-copilot-container';
            container.innerHTML = html;
            document.body.appendChild(container);

            // The partial view contains a <script> tag. 
            // When setting innerHTML, scripts are not executed. 
            // We need to manually execute the script or extract the functions.
            const scriptElements = container.querySelectorAll('script');
            scriptElements.forEach(oldScript => {
                const newScript = document.createElement('script');
                newScript.textContent = oldScript.textContent;
                document.body.appendChild(newScript);
            });

            console.log("AI Copilot loaded successfully.");
        } catch (err) {
            console.error("Error loading AI Copilot:", err);
        }
    }

    // Wait a bit for the backoffice to settle
    setTimeout(loadCopilot, 2000);
})();
