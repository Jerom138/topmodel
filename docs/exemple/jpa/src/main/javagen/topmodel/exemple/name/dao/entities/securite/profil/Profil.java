////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////

package topmodel.exemple.name.dao.entities.securite.profil;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

import javax.annotation.Generated;
import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.OneToMany;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;

import topmodel.exemple.name.dao.entities.securite.profil.TypeProfil;
import topmodel.exemple.name.dao.entities.securite.utilisateur.Utilisateur;
import topmodel.exemple.utils.IFieldEnum;

/**
 * Profil des utilisateurs.
 */
@Generated("TopModel : https://github.com/klee-contrib/topmodel")
@Entity
@Table(name = "PROFIL")
public class Profil {

	/**
	 * Id technique.
	 */
	@Id
	@SequenceGenerator(name = "SEQ_PROFIL", sequenceName = "SEQ_PROFIL", initialValue = 1000)
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_PROFIL")
	@Column(name = "PRO_ID", nullable = false)
	private long id;

	/**
	 * Type de profil.
	 */
	@ManyToOne(fetch = FetchType.LAZY, optional = true, targetEntity = TypeProfil.class)
	@JoinColumn(name = "CODE", referencedColumnName = "CODE")
	private TypeProfil typeProfil;

	/**
	 * Association réciproque de {@link topmodel.exemple.name.dao.entities.securite.utilisateur.Utilisateur#profil Utilisateur.profil}.
	 */
	@OneToMany(cascade = CascadeType.ALL, orphanRemoval = true, fetch = FetchType.LAZY, mappedBy = "profil")
	private List<Utilisateur> utilisateurs;

	/**
	 * No arg constructor.
	 */
	public Profil() {
	}

	/**
	 * Copy constructor.
	 * @param profil to copy
	 */
	public Profil(Profil profil) {
		if(profil == null) {
			return;
		}

		this.id = profil.getId();
		this.typeProfil = profil.getTypeProfil();

		this.utilisateurs = profil.getUtilisateurs().stream().collect(Collectors.toList());
	}

	/**
	 * All arg constructor.
	 * @param id Id technique
	 * @param typeProfil Type de profil
	 * @param utilisateurs Association réciproque de {@link topmodel.exemple.name.dao.entities.securite.utilisateur.Utilisateur#profil Utilisateur.profil}
	 */
	public Profil(long id, TypeProfil typeProfil, List<Utilisateur> utilisateurs) {
		this.id = id;
		this.typeProfil = typeProfil;
		this.utilisateurs = utilisateurs;
	}

	/**
	 * Crée une nouvelle instance de 'Profil'.
	 * @param profil Instance de 'Profil'.
	 *
	 * @return Une nouvelle instance de 'Profil'.
	 */
	public Profil(Profil profil) {
		this.from(profil);
	}

	/**
	 * Map les champs des classes passées en paramètre dans l'instance courante.
	 * @param profil Instance de 'Profil'.
	 */
	protected void from(Profil profil) {
		if(profil != null) {
			this.id = profil.getId();
			this.typeProfil = profil.getTypeProfil();
		}

	}

	/**
	 * Getter for id.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.entities.securite.profil.Profil#id id}.
	 */
	public long getId() {
		return this.id;
	}

	/**
	 * Getter for typeProfil.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.entities.securite.profil.Profil#typeProfil typeProfil}.
	 */
	protected TypeProfil getTypeProfil() {
		return this.typeProfil;
	}

	/**
	 * Getter for utilisateurs.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.entities.securite.profil.Profil#utilisateurs utilisateurs}.
	 */
	public List<Utilisateur> getUtilisateurs() {
		if(this.utilisateurs == null)
			this.utilisateurs = new ArrayList<>();
		return this.utilisateurs;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.entities.securite.profil.Profil#id id}.
	 * @param id value to set
	 */
	public void setId(long id) {
		this.id = id;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.entities.securite.profil.Profil#typeProfil typeProfil}.
	 * @param typeProfil value to set
	 */
	public void setTypeProfil(TypeProfil typeProfil) {
		this.typeProfil = typeProfil;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.entities.securite.profil.Profil#utilisateurs utilisateurs}.
	 * @param utilisateurs value to set
	 */
	public void setUtilisateurs(List<Utilisateur> utilisateurs) {
		this.utilisateurs = utilisateurs;
	}

	/**
	 * Equal function comparing Id.
	 */
	public boolean equals(Object o) {
		if(o instanceof Profil profil) {
			if(this == profil)
				return true;

			if(profil == null || this.getId() == null)
				return false;

			return this.getId().equals(profil.getId());
		}
		return false;
	}

	/**
	 * Mappe 'Profil' vers 'Profil'.
	 * @param source Instance de 'Profil'.
	 * @param dest Instance pré-existante de 'Profil'. Une nouvelle instance sera créée si non spécifié.
	 *
	 * @return Une instance de 'Profil'.
	 */
	public Profil toProfil(Profil dest) {
		dest = dest == null ? new Profil() : dest;

		dest.setId(this.getId());
		dest.setTypeProfil(this.getTypeProfil());

		return dest;
	}

	/**
	 * Enumération des champs de la classe {@link topmodel.exemple.name.dao.entities.securite.profil.Profil Profil}.
	 */
	public enum Fields implements IFieldEnum<Profil> {
        ID, //
        TYPE_PROFIL, //
        UTILISATEURS
	}
}
