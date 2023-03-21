////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////

package topmodel.jpa.sample.demo.dtos.securite;

import java.io.Serializable;
import java.util.List;
import java.util.stream.Collectors;

import jakarta.annotation.Generated;

import topmodel.jpa.sample.demo.dtos.utilisateur.UtilisateurDto;
import topmodel.jpa.sample.demo.entities.securite.Droit;
import topmodel.jpa.sample.demo.entities.securite.Profil;
import topmodel.jpa.sample.demo.entities.securite.SecuriteMappers;
import topmodel.jpa.sample.demo.entities.securite.TypeProfil;

/**
 * Objet métier non persisté représentant Profil.
 */
@Generated("TopModel : https://github.com/klee-contrib/topmodel")
public class ProfilDto implements Serializable {
	/** Serial ID */
	private static final long serialVersionUID = 1L;

	/**
	 * Id technique.
	 * Alias of {@link topmodel.jpa.sample.demo.entities.securite.Profil#getId() Profil#getId()} 
	 */
	private Long id;

	/**
	 * Type de profil.
	 * Alias of {@link topmodel.jpa.sample.demo.entities.securite.Profil#getTypeProfilCode() Profil#getTypeProfilCode()} 
	 */
	private TypeProfil.Values typeProfilCode;

	/**
	 * Liste des droits de l'utilisateur.
	 * Alias of {@link topmodel.jpa.sample.demo.entities.securite.Profil#getDroits() Profil#getDroits()} 
	 */
	private List<Droit.Values> droits;

	/**
	 * Liste paginée des utilisateurs de ce profil.
	 */
	private List<UtilisateurDto> utilisateurs;

	/**
	 * Liste des secteurs du profil.
	 */
	private List<SecteurDto> secteurs;

	/**
	 * No arg constructor.
	 */
	public ProfilDto() {
	}

	/**
	 * Copy constructor.
	 * @param profilDto to copy
	 */
	public ProfilDto(ProfilDto profilDto) {
		if(profilDto == null) {
			return;
		}

		this.id = profilDto.getId();
		this.typeProfilCode = profilDto.getTypeProfilCode();
		this.droits = profilDto.getDroits();

		this.utilisateurs = profilDto.getUtilisateurs().stream().collect(Collectors.toList());
		this.secteurs = profilDto.getSecteurs().stream().collect(Collectors.toList());
	}

	/**
	 * All arg constructor.
	 * @param id Id technique
	 * @param typeProfilCode Type de profil
	 * @param droits Liste des droits de l'utilisateur
	 * @param utilisateurs Liste paginée des utilisateurs de ce profil
	 * @param secteurs Liste des secteurs du profil
	 */
	public ProfilDto(Long id, TypeProfil.Values typeProfilCode, List<Droit.Values> droits, List<UtilisateurDto> utilisateurs, List<SecteurDto> secteurs) {
		this.id = id;
		this.typeProfilCode = typeProfilCode;
		this.droits = droits;
		this.utilisateurs = utilisateurs;
		this.secteurs = secteurs;
	}

	/**
	 * Crée une nouvelle instance de 'ProfilDto'.
	 * @param profil Instance de 'Profil'.
	 *
	 * @return Une nouvelle instance de 'ProfilDto'.
	 */
	public ProfilDto(Profil profil) {
		SecuriteMappers.createProfilDto(profil, this);
	}

	/**
	 * Getter for id.
	 *
	 * @return value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#id id}.
	 */
	public Long getId() {
		return this.id;
	}

	/**
	 * Getter for typeProfilCode.
	 *
	 * @return value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#typeProfilCode typeProfilCode}.
	 */
	public TypeProfil.Values getTypeProfilCode() {
		return this.typeProfilCode;
	}

	/**
	 * Getter for droits.
	 *
	 * @return value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#droits droits}.
	 */
	public List<Droit.Values> getDroits() {
		return this.droits;
	}

	/**
	 * Getter for utilisateurs.
	 *
	 * @return value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#utilisateurs utilisateurs}.
	 */
	public List<UtilisateurDto> getUtilisateurs() {
		return this.utilisateurs;
	}

	/**
	 * Getter for secteurs.
	 *
	 * @return value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#secteurs secteurs}.
	 */
	public List<SecteurDto> getSecteurs() {
		return this.secteurs;
	}

	/**
	 * Set the value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#id id}.
	 * @param id value to set
	 */
	public void setId(Long id) {
		this.id = id;
	}

	/**
	 * Set the value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#typeProfilCode typeProfilCode}.
	 * @param typeProfilCode value to set
	 */
	public void setTypeProfilCode(TypeProfil.Values typeProfilCode) {
		this.typeProfilCode = typeProfilCode;
	}

	/**
	 * Set the value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#droits droits}.
	 * @param droits value to set
	 */
	public void setDroits(List<Droit.Values> droits) {
		this.droits = droits;
	}

	/**
	 * Set the value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#utilisateurs utilisateurs}.
	 * @param utilisateurs value to set
	 */
	public void setUtilisateurs(List<UtilisateurDto> utilisateurs) {
		this.utilisateurs = utilisateurs;
	}

	/**
	 * Set the value of {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto#secteurs secteurs}.
	 * @param secteurs value to set
	 */
	public void setSecteurs(List<SecteurDto> secteurs) {
		this.secteurs = secteurs;
	}

	/**
	 * Mappe 'ProfilDto' vers 'Profil'.
	 * @param target Instance pré-existante de 'Profil'. Une nouvelle instance sera créée si non spécifié.
	 *
	 * @return Une instance de 'Profil'.
	 */
	public Profil toProfil(Profil target) {
		return SecuriteMappers.toProfil(this, target);
	}

	/**
	 * Enumération des champs de la classe {@link topmodel.jpa.sample.demo.dtos.securite.ProfilDto ProfilDto}.
	 */
	public enum Fields  {
        ID(Long.class), //
        TYPE_PROFIL_CODE(TypeProfil.Values.class), //
        DROITS(List.class), //
        UTILISATEURS(List.class), //
        SECTEURS(List.class);

		private Class<?> type;

		private Fields(Class<?> type) {
			this.type = type;
		}

		public Class<?> getType() {
			return this.type;
		}
	}
}
