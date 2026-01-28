document.addEventListener('DOMContentLoaded', async () => {
  await requireAuth();
  const tbody = document.getElementById('tbodyStudentScore');
  const form = document.getElementById('formStudentScore');
  const btnReset = document.getElementById('btnReset');

  const qInput = document.getElementById('q');
  const searchForm = document.getElementById('searchForm');
  const prevBtn = document.getElementById('prevPage');
  const nextBtn = document.getElementById('nextPage');
  const pageInfo = document.getElementById('pageInfo');
  let page = 1;
  const pageSize = 10;
  let total = 0;

  async function load() {
    const q = encodeURIComponent(qInput.value || '');
    const res = await apiFetch(`/api/v1/studentscores?q=${q}&page=${page}&pageSize=${pageSize}`);
    total = res.total;
    const items = res.items;
    tbody.innerHTML = '';
    for (const s of items) {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${s.Id}</td>
        <td>${s.NombreCurso}</td>
        <td>${s.NombreStudent}</td>
		<td>${s.FechaEvaluacion || ''}</td>
		<td>${s.PuntajeObtenido}</td>
        <td>${s.Observacinoes}</td>

        <td>
          <button data-edit="${s.Id}">Editar</button>
          <button data-del="${s.Id}">Borrar</button>
        </td>`;
      tbody.appendChild(tr);
    }
    const totalPages = Math.max(1, Math.ceil(total / pageSize));
    pageInfo.textContent = `Página ${page} de ${totalPages} — ${total} registros`;
    prevBtn.disabled = page <= 1;
    nextBtn.disabled = page >= totalPages;
  }

  tbody.addEventListener('click', async (e) => {
    const idEdit = e.target.getAttribute('data-edit');
    const idDel = e.target.getAttribute('data-del');
    if (idEdit) {
      const s = await apiFetch('/api/v1/studentscores/' + idEdit);
	  document.getElementById('id').value = s.Id;
      document.getElementById('evaluacion').value = s.AssessmentId;
      document.getElementById('inscripcion').value = s.EnrollmentId;
	  document.getElementById('puntaje').value = s.PuntajeObtenido;
	  document.getElementById('observacion').value = s.Observaciones;

    }
    if (idDel) {
      if (!confirm('¿Borrar inscripcion? #' + idDel + '?')) return;
      await apiFetch('/api/v1/studentscores/' + idDel, { method: 'DELETE' });
      await load();
    }
  });

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const payload = {
      assessmentId: document.getElementById('evaluacion').value.trim(),
	  enrollmentId: document.getElementById('inscripcion').value.trim(),
      puntajeObtenido: document.getElementById('puntaje').value.trim(),
      Observacinoes: document.getElementById('observacion').value.trim(),
	  id : 1
    };

    const id = document.getElementById('id').value;
    if (id) {
      await apiFetch('/api/v1/studentscores/' + id, { method: 'PUT', body: JSON.stringify(payload) });
    } else {
      await apiFetch('/api/v1/studentscores', { method: 'POST', body: JSON.stringify(payload) });
    }
    form.reset();
    await load();
  });

  btnReset.addEventListener('click', () => form.reset());
  searchForm.addEventListener('submit', (e) => { e.preventDefault(); page = 1; load(); });
  prevBtn.addEventListener('click', () => { if (page > 1) { page--; load(); } });
  nextBtn.addEventListener('click', () => { page++; load(); });

  await load();
});
