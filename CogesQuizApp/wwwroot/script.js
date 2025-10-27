// === INDEX.HTML ===
if (window.location.pathname.endsWith("index.html") || window.location.pathname === "/") {
    fetch("http://localhost:8080/tests")
        .then(res => res.json())
        .then(tests => {
            const container = document.getElementById("quizList");
            container.innerHTML = "";
            tests.forEach(t => {
                const div = document.createElement("div");
                div.className = "quiz";
                div.innerHTML = `<h3>${t.Title}</h3>`;
                div.onclick = () => window.location.href = `quiz.html?id=${t.Id || t._id}`;
                container.appendChild(div);
            });
        });
}

// === QUIZ.HTML ===
if (window.location.pathname.endsWith("quiz.html")) {
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");
    const username = localStorage.getItem("username") || "Anonimo";

    let testData = null;
    let currentQuestion = 0;
    let score = 0;
    let selectedAnswer = null;

    const questionContainer = document.getElementById("questions");
    const nextButton = document.getElementById("nextQuestion");
    const quizTitle = document.getElementById("quizTitle");

    fetch(`http://localhost:8080/tests/${id}`)
        .then(res => res.json())
        .then(test => {
            testData = test;
            quizTitle.textContent = test.Title;
            showQuestion();
        });

    function showQuestion() {
        selectedAnswer = null;
        nextButton.style.display = "none";

        const q = testData.Questions[currentQuestion];
        questionContainer.innerHTML = `
            <div class="question">
                <p><b>${q.Text}</b></p>
                ${q.Answers.map((a, ai) => `
                    <button type="button" class="answer-btn" data-index="${ai}">${a.Text}</button>
                `).join("")}
            </div>
        `;

        document.querySelectorAll(".answer-btn").forEach(btn => {
            btn.addEventListener("click", () => selectAnswer(btn));
        });
    }

    function selectAnswer(button) {
        // 🔹 evidenzia il bottone selezionato
        document.querySelectorAll(".answer-btn").forEach(b => b.classList.remove("selected"));
        button.classList.add("selected");
        selectedAnswer = parseInt(button.dataset.index);
        nextButton.style.display = "inline-block";
    }

    function checkAnswer() {
        const q = testData.Questions[currentQuestion];
        // ✅ compatibilità con maiuscole/minuscole
        const correctIndex = q.CorrectAnswerIndex ?? q.correctAnswerIndex;
        console.log("👉 selected:", selectedAnswer, "correct:", correctIndex);
        if (selectedAnswer === correctIndex) score++;
    }

    nextButton.addEventListener("click", () => {
        if (selectedAnswer === null) {
            alert("Seleziona una risposta prima di continuare!");
            return;
        }

        checkAnswer();
        currentQuestion++;

        if (currentQuestion < testData.Questions.length) {
            showQuestion();
        } else {
            endQuiz();
        }
    });

    function endQuiz() {
        const total = testData.Questions.length;
        const resultMsg = `Hai risposto correttamente a ${score} su ${total}! 🎯`;
        alert(resultMsg);

        fetch("http://localhost:8080/results", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                testId: id,
                username,
                score,
                totalQuestions: total
            })
        });

        localStorage.removeItem("username");
        window.location.href = "index.html";
    }
}
