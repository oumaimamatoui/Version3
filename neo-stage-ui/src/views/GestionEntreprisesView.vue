<template>
  <div class="p-4">
    <div class="bg-dark text-white p-5 rounded-5 mb-5 shadow-lg">
      <h1>Master Control</h1>
      <p class="opacity-75">Gérez les instances entreprises de la plateforme.</p>
    </div>

    <div class="table-responsive bg-white p-4 rounded-4 shadow-sm">
      <table class="table align-middle">
        <thead>
          <tr>
            <th>Entreprise</th>
            <th>Admin Principal</th>
            <th>Activités</th>
            <th>Statut</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td><strong>TechCorp Tunisia</strong></td>
            <td>Ali Ben Salem</td>
            <td>12 Campagnes</td>
            <td><span class="badge bg-success">Actif</span></td>
            <td><button class="btn btn-sm btn-outline-dark">Désactiver</button></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>