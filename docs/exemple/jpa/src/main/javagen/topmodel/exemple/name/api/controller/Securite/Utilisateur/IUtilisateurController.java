////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////

package topmodel.exemple.name.api.controller.securite.utilisateur;

import java.util.List;
import java.util.stream.Collectors;

import javax.annotation.Generated;
import javax.validation.constraints.Email;
import javax.validation.Valid;

import org.springframework.data.domain.Page;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;

import topmodel.exemple.name.dao.dtos.utilisateur.UtilisateurDto;
import topmodel.exemple.name.dao.entities.utilisateur.TypeUtilisateur;

@Generated("TopModel : https://github.com/klee-contrib/topmodel")
public interface IUtilisateurController {


	/**
	 * Charge le détail d'un utilisateur.
	 * @param utiId Id technique
	 * @return Le détail de l'utilisateur
	 */
	@GetMapping(path = "utilisateur/{utilisateurId}")
	UtilisateurDto getUtilisateur(@RequestParam(value = "utiId", required = true) long utiId);

	/**
	 * Charge une liste d'utilisateurs par leur type.
	 * @param typeUtilisateurCode Type d'utilisateur en Many to one
	 * @return Liste des utilisateurs
	 */
	@GetMapping(path = "utilisateur/list")
	List<UtilisateurDto> getUtilisateurList(@RequestParam(value = "typeUtilisateurCode", required = false) TypeUtilisateur.Values typeUtilisateurCode);

	/**
	 * Sauvegarde un utilisateur.
	 * @param utilisateur Utilisateur à sauvegarder
	 * @return Utilisateur sauvegardé
	 */
	@PostMapping(path = "utilisateur/save")
	UtilisateurDto saveUtilisateur(@RequestBody @Valid UtilisateurDto utilisateur);

	/**
	 * Sauvegarde une liste d'utilisateurs.
	 * @param utilisateur Utilisateur à sauvegarder
	 * @return Utilisateur sauvegardé
	 */
	@PostMapping(path = "utilisateur/saveAll")
	List<UtilisateurDto> saveAllUtilisateur(@RequestBody @Valid List<UtilisateurDto> utilisateur);

	/**
	 * Recherche des utilisateurs.
	 * @param utiUtilisateurId Id technique
	 * @param utilisateurEmail Email de l'utilisateur
	 * @param utilisateurTypeUtilisateurCode Type d'utilisateur en Many to one
	 * @return Utilisateurs matchant les critères
	 */
	@PostMapping(path = "utilisateur/search")
	Page<UtilisateurDto> search(@RequestParam(value = "utiUtilisateurId", required = true) long utiUtilisateurId, @RequestParam(value = "utilisateurEmail", required = true) String utilisateurEmail, @RequestParam(value = "utilisateurTypeUtilisateurCode", required = true) TypeUtilisateur.Values utilisateurTypeUtilisateurCode);
}
