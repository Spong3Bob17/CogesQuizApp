
// ============================================
// SEZIONE INDEX.HTML
// ============================================
// Gestisce la homepage dove l'utente seleziona un test

if (window.location.pathname.endsWith("index.html") || window.location.pathname === "/") {
    // Carica la lista dei test disponibili dal server
    fetch("http://localhost:8080/tests")
        .then(res => res.json())
        .then(tests => {
            // Ottiene il container dove mostrare i test
            const container = document.getElementById("quizList");
            container.innerHTML = "";

            // Crea un elemento per ogni test disponibile
            tests.forEach(t => {
                const div = document.createElement("div");
                div.className = "quiz";
                div.innerHTML = `<h3>${t.Title}</h3>`;

                // Al click, reindirizza alla pagina del quiz con l'ID del test
                div.onclick = () => window.location.href = `quiz.html?id=${t.Id || t._id}`;

                container.appendChild(div);
            });
        })
        .catch(error => {
            console.error("Errore nel caricamento dei test:", error);
        });
}

// ============================================
// SEZIONE QUIZ.HTML
// ============================================
// Gestisce l'esecuzione del quiz e il salvataggio delle risposte

if (window.location.pathname.endsWith("quiz.html")) {
    // ============================================
    // Variabili Globali
    // ============================================

    // Estrae i parametri dall'URL (id del test)
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");

    // Recupera il nome utente dal localStorage o usa "Anonimo"
    const username = localStorage.getItem("username") || "Anonimo";

    // Variabili di stato del quiz
    let testData = null;           // Dati del test caricati dal server
    let currentQuestion = 0;       // Indice della domanda corrente (0-based)
    let score = 0;                 // Punteggio corrente (risposte corrette)
    let selectedAnswer = null;     // Indice della risposta selezionata dall'utente

    // ============================================
    // Riferimenti agli Elementi DOM
    // ============================================
    const questionContainer = document.getElementById("questions");
    const nextButton = document.getElementById("nextQuestion");
    const quizTitle = document.getElementById("quizTitle");

    // ============================================
    // Caricamento Test dal Server
    // ============================================

    // Effettua una richiesta al server per ottenere i dati del test
    fetch(`http://localhost:8080/tests/${id}`)
        .then(res => res.json())
        .then(test => {
            // Salva i dati del test
            testData = test;

            // Mostra il titolo del test
            quizTitle.textContent = test.Title;

            // Mostra la prima domanda
            showQuestion();
        })
        .catch(error => {
            console.error("Errore nel caricamento del test:", error);
            alert("Impossibile caricare il test. Torna alla homepage.");
            window.location.href = "index.html";
        });

    // ============================================
    // Funzione: Mostra Domanda Corrente
    // ============================================

    /**
     * Mostra la domanda corrente con le sue opzioni di risposta.
     * Reset lo stato della selezione e nasconde il bottone "Next".
     */
    function showQuestion() {
        // Reset stato
        selectedAnswer = null;
        nextButton.style.display = "none";

        // Ottiene la domanda corrente dall'array
        const q = testData.Questions[currentQuestion];

        // Costruisce l'HTML della domanda e delle risposte
        questionContainer.innerHTML = `
            <div class="question">
                <p><b>${q.Text}</b></p>
                ${q.Answers.map((a, ai) => `
                    <button type="button" class="answer-btn" data-index="${ai}">${a.Text}</button>
                `).join("")}
            </div>
        `;

        // Aggiunge event listener a tutti i bottoni di risposta
        document.querySelectorAll(".answer-btn").forEach(btn => {
            btn.addEventListener("click", () => selectAnswer(btn));
        });
    }

    // ============================================
    // Funzione: Selezione Risposta
    // ============================================

    /**
     * Gestisce la selezione di una risposta da parte dell'utente.
     * Evidenzia visivamente la risposta selezionata e mostra il bottone Next.
     *
     * @param {HTMLElement} button - Il bottone della risposta cliccato
     */
    function selectAnswer(button) {
        // Rimuove la classe "selected" da tutti i bottoni
        document.querySelectorAll(".answer-btn").forEach(b => b.classList.remove("selected"));

        // Aggiunge la classe "selected" al bottone cliccato
        button.classList.add("selected");

        // Salva l'indice della risposta selezionata
        selectedAnswer = parseInt(button.dataset.index);

        // Mostra il bottone per andare alla prossima domanda
        nextButton.style.display = "inline-block";
    }

    // ============================================
    // Funzione: Verifica Risposta
    // ============================================

    /**
     * Verifica se la risposta selezionata è corretta.
     * Confronta l'indice selezionato con quello della risposta corretta.
     */
    function checkAnswer() {
        const q = testData.Questions[currentQuestion];

        // Supporta sia CorrectAnswerIndex che correctAnswerIndex (compatibilità)
        const correctIndex = q.CorrectAnswerIndex ?? q.correctAnswerIndex;

        console.log("Risposta selezionata:", selectedAnswer, "Risposta corretta:", correctIndex);

        // Se la risposta è corretta, incrementa il punteggio
        if (selectedAnswer === correctIndex) {
            score++;
        }
    }

    // ============================================
    // Event Listener: Bottone "Next"
    // ============================================

    nextButton.addEventListener("click", () => {
        // Verifica che sia stata selezionata una risposta
        if (selectedAnswer === null) {
            alert("Seleziona una risposta prima di continuare!");
            return;
        }

        // Verifica se la risposta è corretta
        checkAnswer();

        // Passa alla prossima domanda
        currentQuestion++;

        // Se ci sono ancora domande, mostra la prossima
        if (currentQuestion < testData.Questions.length) {
            showQuestion();
        } else {
            // Altrimenti, termina il quiz
            endQuiz();
        }
    });

    // ============================================
    // Funzione: Termina Quiz
    // ============================================

    /**
     * Gestisce la fine del quiz.
     * Salva il risultato finale nel database e mostra il punteggio all'utente.
     */
    function endQuiz() {
        const total = testData.Questions.length;
        const resultMsg = `Hai risposto correttamente a ${score} su ${total}! 🎯`;

        // Mostra il risultato all'utente
        alert(resultMsg);

        // Prepara i dati del risultato da inviare al server
        const resultData = {
            testId: id,
            username: username,
            score: score,
            totalQuestions: total
        };

        // Invia il risultato al server tramite POST
        fetch("http://localhost:8080/results", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(resultData)
        })
            .then(response => {
                if (response.ok) {
                    console.log("Risultato salvato con successo");
                } else {
                    console.error("Errore nel salvataggio del risultato");
                }
            })
            .catch(error => {
                console.error("Errore durante il salvataggio:", error);
            });

        // Rimuove il nome utente dal localStorage
        localStorage.removeItem("username");

        // Reindirizza alla homepage
        window.location.href = "index.html";
    }
}